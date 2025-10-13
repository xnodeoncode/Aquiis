using Microsoft.Extensions.Options;

namespace Aquiis.WebUI.Components.Administration.Application
{
    public class ApplicationService
    {
        private readonly ApplicationSettings _settings;
        public bool SoftDeleteEnabled { get; }

        public ApplicationService(IOptions<ApplicationSettings> settings)
        {
            _settings = settings.Value;
            SoftDeleteEnabled = _settings.SoftDeleteEnabled;
        }

        public string GetAppInfo()
        {
            return $"{_settings.AppName} - {_settings.Version}";
        }
    }
}