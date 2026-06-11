namespace CeriOS.LowCodeForm.BasicApi.ConfigurableOptions.Options
{
    /// <summary>
    /// 应用选项依赖接口
    /// </summary>
    public partial interface IConfigurableOptions { }

    public partial interface IConfigurableOptions<TOptions> : IConfigurableOptions
    where TOptions : class, IConfigurableOptions
    {
        /// <summary>
        /// 选项后期配置
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        void PostConfigure(TOptions options, IConfiguration configuration);
    }
}
