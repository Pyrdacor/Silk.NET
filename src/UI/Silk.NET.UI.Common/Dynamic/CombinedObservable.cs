using System.Linq;
using System;
using System.Collections.Generic;

namespace Silk.NET.UI
{
    internal enum CombinedType
    {
        Any,
        All
    }

    /// <summary>
    /// Creates a combined observable which react to changes of
    /// its child observables.
    /// </summary>
    /// <typeparam name="T">Value type of child observables</typeparam>
    /// <typeparam name="U">Combined value type (e.g. an array like T[])</typeparam>
    internal class CombinedObservable<T, U> : Observable<U>, IObservableStatusProvider
    {
        private enum ObservableState
        {
            Empty, // no next or complete yet
            Error,
            HasValue, // at least one next was called
            Completed, // completed with value
            CompletedEmpty // completed without a value
        }

        private class ObservableInfo
        {
            public ObservableState State;
            public T Value;
        }

        private readonly Dictionary<Observable<T>, ObservableInfo> observables;
        private readonly Dictionary<Observable<T>, Subscription<T>> subscriptions = new Dictionary<Observable<T>, Subscription<T>>();
        private Func<IEnumerable<T>, U> mapper;
        private Exception mostRecentError = null;
        private bool replay = false;
        private bool allowErrors = false;
        private bool oneValuePerObservale = false;
        private bool hasValue = false;
        private bool errored = false;
        private bool completed = false;
        private CombinedType combinedNextType;
        private CombinedType combinedCompleteType;
        private U lastValue = default(U);

        bool IObservableStatusProvider.HasValue => hasValue;
        bool IObservableStatusProvider.Errored => errored;
        bool IObservableStatusProvider.Completed => completed;
        internal U LastValue => lastValue;

        /// <summary>
        /// Create a wrapper observable that handles a list of observables.
        /// </summary>
        /// <param name="combinedNextType">Any: Emit when any observable emits. All: Emit when all observables have emitted once.</param>
        /// <param name="combinedCompleteType">Any: Complete when any observable completes. All: Complete when all observables are completed.</param>
        /// <param name="replay">Emit for every new subscriber if not completed and a value is present.</param>
        /// <param name="allowErrors">If false any error will immediately error and complete the observable.</param>
        /// <param name="oneValuePerObservale">If true after first emission of an observable, value subscription to that observable ends.</param>
        /// <param name="mapper">Mapping function to convert a list of inputs to the desired output.</param>
        /// <param name="observables">Input observables</param>
        public CombinedObservable(CombinedType combinedNextType, CombinedType combinedCompleteType, bool replay, bool allowErrors,
            bool oneValuePerObservale, Func<IEnumerable<T>, U> mapper, params Observable<T>[] observables)
        {
            if (observables.Length == 0)
            {
                completed = true;
                return;
            }

            this.mapper = mapper;
            this.combinedNextType = combinedNextType;
            this.combinedCompleteType = combinedCompleteType;
            this.replay = replay;
            this.allowErrors = allowErrors;
            this.oneValuePerObservale = oneValuePerObservale;
            this.observables = new Dictionary<Observable<T>, ObservableInfo>(observables.Length);

            foreach (var observable in observables)
            {
                var state = ObservableState.Empty;

                if (observable is IObservableStatusProvider)
                {
                    var observableWithStatus = observable as IObservableStatusProvider;
                    
                    if (observableWithStatus.Errored)
                    {
                        if (allowErrors)
                            state = ObservableState.Error;
                        else
                        {
                            completed = true;
                            return;
                        }
                    }
                    else if (observableWithStatus.HasValue)
                        state = observableWithStatus.Completed ? ObservableState.Completed : ObservableState.HasValue;
                    else if (observableWithStatus.Completed)
                        state = ObservableState.CompletedEmpty;
                }
                
                this.observables.Add(observable, new ObservableInfo() { State = state });
                subscriptions.Add(observable, observable.Subscribe(
                    value => {
                        var observableInfo = this.observables[observable];
                        observableInfo.Value = value;
                        if (oneValuePerObservale)
                        {
                            observableInfo.State = ObservableState.Completed;
                            if (subscriptions.ContainsKey(observable))
                            {
                                subscriptions[observable].Unsubscribe();
                                subscriptions.Remove(observable);
                            }
                        }
                        else if (observableInfo.State == ObservableState.Empty)
                            observableInfo.State = ObservableState.HasValue;
                        UpdateState();
                    },
                    error => {
                        mostRecentError = error;
                        this.observables[observable].State = ObservableState.Error;
                        UpdateState();
                    },
                    () => {
                        var observableInfo = this.observables[observable];
                        if (observableInfo.State != ObservableState.Error)
                            observableInfo.State = observableInfo.State == ObservableState.HasValue ? ObservableState.Completed : ObservableState.CompletedEmpty;
                        UpdateState();
                    }
                ));
                if (subscriptions.ContainsKey(observable) && subscriptions[observable] == Subscription<T>.Empty)
                    subscriptions.Remove(observable);
            }
        }

        public override Subscription<U> Subscribe(Action<U> next, Action<Exception> error = null, Action complete = null)
        {
            if (completed)
                return Subscription<U>.Empty;

            if (observables.Count == 0)
            {
                // This case happens if an error exists already
                // in the starting observables and errors are not allowed.
                // As there is no subscription on creating the observable
                // we will wait for the first subscription, pass the error
                // to the first subscriber and complete the observable.
                error?.Invoke(mostRecentError);
                completed = true;
                return Subscription<U>.Empty;
            }

            var subscription = base.Subscribe(next, error, complete);

            if (replay && hasValue)
                CallNextActions(lastValue);

            return subscription;
        }

        private void UpdateState()
        {
            int numWithValue = 0;
            int numCompleted = 0;

            foreach (var observable in observables)
            {
                var state = observable.Value.State;

                if (state == ObservableState.Error)
                {
                    if (!allowErrors)
                    {
                        this.hasValue = false;
                        this.errored = true;
                        this.completed = true;                    
                        CallErrorActions(mostRecentError);
                        Clear();
                        return;
                    }
                    else if (combinedNextType == CombinedType.All && combinedCompleteType == CombinedType.All)
                    {
                        // no need to look any further -> can never call next nor complete with an error
                        return;
                    }
                }

                if (numWithValue != -1)
                {
                    bool hasValue = state == ObservableState.HasValue || state == ObservableState.Completed;

                    if (combinedNextType == CombinedType.All && !hasValue)
                        numWithValue = -1; // no further checking
                    else if (hasValue)
                    {
                        if (combinedNextType == CombinedType.All)
                            ++numWithValue;
                        else
                            numWithValue = int.MaxValue; // no further checking needed
                    }
                }

                if (numCompleted == -1)
                {
                    bool completed = state == ObservableState.Completed || state == ObservableState.CompletedEmpty;

                    if (combinedCompleteType == CombinedType.All && !completed)
                        numCompleted = -1; // no further checking
                    else if (completed)
                    {
                        if (combinedCompleteType == CombinedType.All)
                            ++numCompleted;
                        else
                            numCompleted = int.MaxValue; // no further checking needed
                    }
                }

                if ((numWithValue == -1 || numWithValue == int.MaxValue) && (numCompleted == -1 || numCompleted == int.MaxValue))
                    break; // no need to look any further
            }

            if (numWithValue >= observables.Count)
            {
                lastValue = mapper(observables.Select(o => o.Value.Value));
                hasValue = true;
                CallNextActions(lastValue);
            }

            if (numCompleted >= observables.Count)
            {
                this.completed = true;
                CallCompleteActions();
                Clear();
            }
        }

        private void Clear()
        {
            foreach (var subscription in subscriptions.Values)
                subscription.Unsubscribe();

            subscriptions.Clear();
            observables.Clear();
            lastValue = default(U);
        }
    }
}