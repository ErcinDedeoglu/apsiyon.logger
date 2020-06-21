using Sentry;

namespace Apsiyon.Logger.Helper
{
    public class SentryHelper
    {
        private readonly string _dsn;

        public SentryHelper(string dsn)
        {
            _dsn = dsn;
        }

        public bool Capture(string json)
        {
            var result = false;
            var sentryId = new Sentry.SentryClient(new SentryOptions()
            {
                Dsn = new Dsn(_dsn)
            }).CaptureMessage(json);

            if (!string.IsNullOrWhiteSpace(sentryId.ToString())) result = true;

            return result;
        }
    }
}