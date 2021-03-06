// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Core.Attributes;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Loader;

#pragma warning disable 1591

namespace Silk.NET.OpenGLES.Extensions.EXT
{
    public static class ExtTextureBorderClampOverloads
    {
        public static unsafe void GetSamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetSamplerParameterI(sampler, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetSamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] SamplerParameterI pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetSamplerParameterI(sampler, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetSamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetSamplerParameterI(sampler, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetSamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] SamplerParameterI pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetSamplerParameterI(sampler, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] GetTextureParameter pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] GetTextureParameter pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<int> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] GetTextureParameter pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void GetTexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] GetTextureParameter pname, [Count(Computed = "pname"), Flow(FlowDirection.Out)] Span<uint> @params)
        {
            // SpanOverloader
            thisApi.GetTexParameterI(target, pname, out @params.GetPinnableReference());
        }

        public static unsafe void SamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> param)
        {
            // SpanOverloader
            thisApi.SamplerParameterI(sampler, pname, in param.GetPinnableReference());
        }

        public static unsafe void SamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] SamplerParameterI pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> param)
        {
            // SpanOverloader
            thisApi.SamplerParameterI(sampler, pname, in param.GetPinnableReference());
        }

        public static unsafe void SamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> param)
        {
            // SpanOverloader
            thisApi.SamplerParameterI(sampler, pname, in param.GetPinnableReference());
        }

        public static unsafe void SamplerParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] uint sampler, [Flow(FlowDirection.In)] SamplerParameterI pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> param)
        {
            // SpanOverloader
            thisApi.SamplerParameterI(sampler, pname, in param.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] TextureParameterName pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] TextureParameterName pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<int> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] EXT target, [Flow(FlowDirection.In)] TextureParameterName pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] EXT pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

        public static unsafe void TexParameterI(this ExtTextureBorderClamp thisApi, [Flow(FlowDirection.In)] TextureTarget target, [Flow(FlowDirection.In)] TextureParameterName pname, [Count(Computed = "pname"), Flow(FlowDirection.In)] ReadOnlySpan<uint> @params)
        {
            // SpanOverloader
            thisApi.TexParameterI(target, pname, in @params.GetPinnableReference());
        }

    }
}

