using System;
using System.Globalization;

namespace Harp.Common.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Try to cast the object from one type to another
        /// </summary>
        /// <param name="inputValue">The source object</param>
        /// <param name="type">The target type</param>
        /// <param name="outputValue">The target object</param>
        /// <returns>Returns the result of conversion operation</returns>
        public static bool TryConvert(this object inputValue, Type type, out object outputValue)
        {
            outputValue = null;

            try
            {
                if (inputValue == null) return true;

                var convertType = Nullable.GetUnderlyingType(type) ?? type;
                if (inputValue is string stringValue)
                {
                    if (string.IsNullOrEmpty(stringValue)) return true;

                    if (convertType == typeof(DateTime))
                    {
                        var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                        var parseResult = DateTime.TryParseExact(stringValue, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue);
                        if (!parseResult) return false;

                        outputValue = dateTimeValue;
                        return true;
                    }

                    if (convertType.IsEnum)
                    {
                        outputValue = Enum.Parse(convertType, stringValue);
                        return true;
                    }
                }

                outputValue = Convert.ChangeType(inputValue, convertType);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Casts the object to specified type
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="value">The source object</param>
        /// <returns>Returns the object of specified type</returns>
        public static T To<T>(this object value)
        {
            // Get the type that was made nullable
            Type valueType = Nullable.GetUnderlyingType(typeof(T));

            if (valueType != null)
            {
                // Nullable type
                if (value == null)
                {
                    return default(T);
                }

                // Convert to the value type
                object result = Convert.ChangeType(value, valueType);

                // Cast the value type to the nullable type
                return (T)result;
            }

            // Not nullable
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
