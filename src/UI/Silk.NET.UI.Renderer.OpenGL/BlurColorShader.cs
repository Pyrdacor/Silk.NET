namespace Silk.NET.UI.Renderer.OpenGL
{
    /**
     * For blurring we use a superellipse with exponent 4 (aka rounded rectangle).
     *
     * The formula for such a superellipse with width w and height h around the
     * origin (0,0) is: 16((x/w)⁴+(y/h)⁴) = 1
     *
     * The blur radius (r) specifies the distance between two of those superellipses.
     * The inner one: 16((x/w_i)⁴+(y/h_i)⁴) = 1
     * The outer one: 16((x/w_o)⁴+(y/h_o)⁴) = 1
     * 
     * w_o and h_o are equivalent to the dimensions of the drawn rectangle.
     * w_i is w_o - r
     * h_i is h_o - r
     *
     * The blur factor (alpha) is linear between inner (a=1) and outer (a=0) superellipse.
     *
     * Let's call the formula for the inner superellipse i(x,y) and for the outer one o(x,y).
     * If o(x,y) = 1 the point is on the outer round rect.
     * If i(x,y) = 1 the point is on the outer round rect.
     * If o(x,y) > 1 the point is outside the outer round rect (a=0).
     * If i(x,y) <= 1 the point is inside the inner round rect (a=1).
     * If i(x,y) > 1 and o(x,y) < 1 the point is in the blurred area.
     * 
     * In the latter case we calculate the blur factor as follows:
     *
     * 1. Create a line from origin to vertex position (P)
     *    The form is y = m * x where m = y_P / x_P
     * 2. Check where the line crosses the inner ellipse
     *    x_c = 1 / (2 * sqrt_4((1/w_i)⁴ + (m/h_i)⁴))
     * 3. Insert x_c into the line formula to get y_c -> y_c = m * x_c
     * 4. Calculate the distance between P and (x_c, y_c).
     * 5. Normalize the result: a = dist / blurRadius
     */
    internal class BlurColorShader : ColorShader
    {
        private static BlurColorShader _blurColorShader = null;
        internal static readonly string DefaultScreenHeightName = "screenHeight";
        internal static readonly string DefaultSizeName = "size";
        internal static readonly string DefaultBlurRadiusName = "blurRadius";

        private readonly string _screenHeightName;
        private readonly string _sizeName;
        private readonly string _blurRadiusName;

        private static readonly string[] BlurColorFragmentShader = new string[]
        {
            GetFragmentShaderHeader(),
            $"{GetInName(true)} vec4 pixelColor;",
            $"flat {GetInName(true)} uint screenH;",
            $"flat {GetInName(true)} uint blurR;",
            $"flat {GetInName(true)} ivec2 blurOuterSize;",
            $"",
            $"float get_blur_range(float x, float y, float w, float h)",
            $"{{",
            $"    return 16.0f * (pow(x/w,4.0f) + pow(y/h,4.0f));",
            $"}}",
            $"",
            $"float blur()",
            $"{{",
            $"    if (blurR == 0u)",
            $"        return 1.0f;",
            $"    float r = float(blurR);",
            $"    float w = float(blurOuterSize.x);",
            $"    float h = float(blurOuterSize.y);",
            $"    float x = gl_FragCoord.x - 230.0f - w * 0.5f;", // TODO
            $"    float y = float(screenH) - gl_FragCoord.y - 230.0f - h * 0.5f;", // TODO
            $"    float wi = w - 2.0f * r;",
            $"    float hi = h - 2.0f * r;",
            $"    float inner_range = get_blur_range(x, y, wi, hi);",
            $"    if (inner_range <= 1.0f)",
            $"        return 0.0f;",
            $"    float outer_range = get_blur_range(x, y, w, h);",
            $"    if (outer_range > 1.0f)",
            $"        return 1.0f;",
            $"    //return 1.0f;", // TODO: test
            $"    float xc = 0.0f;",
            $"    float yc = y;",
            $"    if (x != 0)",
            $"    {{",
            $"        float m = y / x;",
            $"        xc = 1.0f / (2.0f * pow(pow(1/wi,4) + pow(m/hi,4), 0.25f));",
            $"        yc = m * xc;",
            $"    }}",
            $"    float dist = sqrt(pow(x-xc,2) + pow(y-yc,2));",
            $"    return dist / r;",
            $"}}",
            $"",
            $"void main()",
            $"{{",
            $"    {(HasGLFragColor() ? "gl_FragColor" : DefaultFragmentOutColorName)} = vec4(pixelColor.rgb, pixelColor.a * blur());",
            $"}}"
        };

        private static readonly string[] BlurColorVertexShader = new string[]
        {
            GetVertexShaderHeader(),
            $"{GetInName(false)} ivec2 {DefaultPositionName};",
            $"{GetInName(false)} ivec2 {DefaultSizeName};",
            $"{GetInName(false)} uint {DefaultLayerName};",
            $"{GetInName(false)} uvec4 {DefaultColorName};",
            $"{GetInName(false)} uint {DefaultBlurRadiusName};",
            $"uniform uint {DefaultScreenHeightName};",
            $"uniform float {DefaultZName};",
            $"uniform mat4 {DefaultProjectionMatrixName};",
            $"uniform mat4 {DefaultModelViewMatrixName};",
            $"{GetOutName()} vec4 pixelColor;",
            $"flat {GetOutName()} uint screenH;",
            $"flat {GetOutName()} uint blurR;",
            $"flat {GetOutName()} ivec2 blurOuterSize;",
            $"",
            $"void main()",
            $"{{",
            $"    vec2 pos = vec2(float({DefaultPositionName}.x) + 0.49f, float({DefaultPositionName}.y) + 0.49f);",
            $"    pixelColor = vec4({DefaultColorName}.r / 255.0f, {DefaultColorName}.g / 255.0f, {DefaultColorName}.b / 255.0f, {DefaultColorName}.a / 255.0f);",
            $"    screenH = {DefaultScreenHeightName};",
            $"    blurR = {DefaultBlurRadiusName};",
            $"    blurOuterSize = {DefaultSizeName};",
            $"    gl_Position = {DefaultProjectionMatrixName} * {DefaultModelViewMatrixName} * vec4(pos, 1.0f - {DefaultZName} - float({DefaultLayerName}) * 0.00001f, 1.0f);",
            $"}}"
        };

        BlurColorShader()
            : this(DefaultModelViewMatrixName, DefaultProjectionMatrixName, DefaultZName, DefaultPositionName, 
                  DefaultSizeName, DefaultBlurRadiusName, DefaultLayerName, DefaultScreenHeightName,
                  BlurColorFragmentShader, BlurColorVertexShader)
        {

        }

        protected BlurColorShader(string modelViewMatrixName, string projectionMatrixName, string zName,
            string positionName, string sizeName, string blurRadiusName, string layerName,
            string screenHeightName, string[] fragmentShaderLines, string[] vertexShaderLines)
            : base(modelViewMatrixName, projectionMatrixName, DefaultColorName, zName, positionName,
                layerName, fragmentShaderLines, vertexShaderLines)
        {
            _screenHeightName = screenHeightName;
            _sizeName = sizeName;
            _blurRadiusName = blurRadiusName;
        }

        public void SetScreenHeight(uint height)
        {
            _shaderProgram.SetInput(_screenHeightName, height);
        }

        public new static BlurColorShader Instance
        {
            get
            {
                if (_blurColorShader == null)
                    _blurColorShader = new BlurColorShader();

                return _blurColorShader;
            }
        }
    }
}
