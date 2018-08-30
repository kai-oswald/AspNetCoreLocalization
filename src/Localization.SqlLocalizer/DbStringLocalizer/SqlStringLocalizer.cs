using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Localization.SqlLocalizer.DbStringLocalizer
{
    public class SqlStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, string> _localizations;
        private readonly string _defaultCulture;
        private readonly DevelopmentSetup _developmentSetup;
        private readonly string _resourceKey;
        private bool _returnKeyOnlyIfNotFound;
        private bool _createNewRecordWhenLocalisedStringDoesNotExist;

        public SqlStringLocalizer(Dictionary<string, string> localizations, DevelopmentSetup developmentSetup, string resourceKey, bool returnKeyOnlyIfNotFound, bool createNewRecordWhenLocalisedStringDoesNotExist)
        {
            _localizations = localizations;
            _developmentSetup = developmentSetup;
            _resourceKey = resourceKey;
            _returnKeyOnlyIfNotFound = returnKeyOnlyIfNotFound;
            _createNewRecordWhenLocalisedStringDoesNotExist = createNewRecordWhenLocalisedStringDoesNotExist;
        }
        public SqlStringLocalizer(string defaultCulture, Dictionary<string, string> localizations, DevelopmentSetup developmentSetup, string resourceKey, bool returnKeyOnlyIfNotFound, bool createNewRecordWhenLocalisedStringDoesNotExist)
        {
            this._defaultCulture = defaultCulture;
            _localizations = localizations;
            _developmentSetup = developmentSetup;
            _resourceKey = resourceKey;
            _returnKeyOnlyIfNotFound = returnKeyOnlyIfNotFound;
            _createNewRecordWhenLocalisedStringDoesNotExist = createNewRecordWhenLocalisedStringDoesNotExist;
        }
        public LocalizedString this[string name]
        {
            get
            {
                bool notSucceed;
                var text = GetText(name, out notSucceed);

                return new LocalizedString(name, text, notSucceed);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return this[name];
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();

        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetText(string key, out bool notSucceed)
        {

#if NET451
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
#elif NET46
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
#else
            var culture = CultureInfo.CurrentCulture.ToString();
#endif
            string result;





            string computedKey = $"{key}.{culture}";


            if (_localizations.TryGetValue(computedKey, out result))
            {
                notSucceed = false;
                return result;
            }
            else
            {
                if (!string.IsNullOrEmpty(_defaultCulture))
                {
                    var defaultCulturekey = $"{key}.{_defaultCulture}";
                    if (_localizations.TryGetValue(defaultCulturekey, out result))
                    {
                        notSucceed = false;
                        return result;
                    }
                }
                notSucceed = true;

                if (_createNewRecordWhenLocalisedStringDoesNotExist)
                {
                    _developmentSetup.AddNewLocalizedItem(key, culture, _resourceKey);
                    _localizations.Add(computedKey, computedKey);
                    return computedKey;
                }
                if (_returnKeyOnlyIfNotFound)
                {
                    return key;
                }


                return _resourceKey + "." + computedKey;
            }
        }




    }
}
