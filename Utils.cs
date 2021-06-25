
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

/// <summary>
/// 
/// </summary>
public static class TN_Utils
{
    #region [Random Number]
    // Instantiate random number generator.  
    // It is better to keep a single Random instance 
    // and keep using Next on the same instance.  
    private static readonly Random _random = new Random();

    // Generates a random number within a range.      
    public static int RandomNumber(int min, int max)
    {
        return _random.Next(min, max);
    }

    // Generates a random string with a given size.    
    public static string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);

        // Unicode/ASCII Letters are divided into two blocks
        // (Letters 65–90 / 97–122):   
        // The first group containing the uppercase letters and
        // the second group containing the lowercase.  

        // char is a single Unicode character  
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26; // A...Z or a..z: length = 26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    // Generates a random password.  
    // 4-LowerCase + 4-Digits + 2-UpperCase  
    public static string RandomPassword()
    {
        var passwordBuilder = new StringBuilder();

        // 4-Letters lower case   
        passwordBuilder.Append(RandomString(4, true));

        // 4-Digits between 1000 and 9999  
        passwordBuilder.Append(RandomNumber(1000, 9999));

        // 2-Letters upper case  
        passwordBuilder.Append(RandomString(2));
        return passwordBuilder.ToString();
    }
    #endregion

    #region [String Helper]    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Input"></param>
    /// <returns></returns>
    public static string Create64String(Stream Input)
    {
        Stream fs = Input;
        BinaryReader br = new BinaryReader(fs);
        Byte[] bytes = br.ReadBytes((Int32)fs.Length);
        string base64String = "data:image/png;base64," + Convert.ToBase64String(bytes, 0, bytes.Length);
        return base64String;
    }

    public static string ToStringComma(this IList<string> data)
    {
        return "'" + string.Join("', '", data) + "'";
    }

    /// <summary>
    /// trả về ngày đầu tuần của ngày chỉ định
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime FirstDayOfWeek(DateTime date)
    {
        DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        int offset = fdow - date.DayOfWeek;
        DateTime fdowDate = date.AddDays(offset);
        return fdowDate;
    }
    /// <summary>
    /// trả về ngày cuối tuấn của ngày chỉ định
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime LastDayOfWeek(DateTime date)
    {
        DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
        return ldowDate;
    }

    /// <summary>Trim all String properties of the given object</summary>
    public static TSelf TrimStringProperties<TSelf>(this TSelf input)
    {
        if (input == null)
        {
            return input;
        }

        var stringProperties = typeof(TSelf).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        foreach (var stringProperty in stringProperties)
        {
            string currentValue = (string)stringProperty.GetValue(input, null);
            if (currentValue != null)
            {
                stringProperty.SetValue(input, currentValue.Trim(), null);
            }
        }
        return input;
    }

    /// <summary>
    /// Lấy dòng đầu tiên của chuỗi, khi chuỗi có nhiều xuống dòng
    /// </summary>
    /// <param name="Input"></param>
    /// <returns></returns>
    public static string GetFirstLine(this string Input)
    {
        return new StringReader(Input).ReadLine();
    }

    /// <summary>
    /// Kiem tra phải là GUID hay không
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public static bool ValidateGuid(string inputString)
    {
        Guid guidOutput;
        return Guid.TryParse(inputString, out guidOutput);
    }
    /// <summary>
    /// Cắt chuỗi nhiều ký tự ví dụ: nhập vào [I'll text you my address 780 Trần Hưng Đạo]
    /// </summary>
    /// <param name="strContent">chuỗi</param>
    /// <param name="Length">độ dài cần lấy</param>
    /// <returns>I'll text you my address 780 ....</returns>
    public static string GetShortContent(string strContent, int Length)
    {
        if (strContent.Length < Length)
        {
            return strContent;
        }
        else
        {
            return (strContent.Substring(0, Length) + "...");
        }
    }

    /// <summary>
    /// Chữ hoa đầu dòng
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToUpper(this string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return input.First().ToString().ToUpper() + input.Substring(1);
    }

    /// <summary>
    /// Chuyển số sang chữ ENGLISH
    /// </summary>
    /// <param name="lNumber"></param>
    /// <returns></returns>
    public static string NumberToWordsEN(long lNumber)
    {

        string[] ones = {"One ","Two ","Three ","Four ","Five ","Six ","Seven ","Eight ","Nine ","Ten ",
                              "Eleven ","Twelve ","Thirteen ","Fourteen ","Fifteen ","Sixteen ","Seventeen ","Eighteen ","Ninteen "
                            };
        string[] tens = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninty " };

        if (lNumber == 0)
        {
            return ("");
        }

        if (lNumber < 0)
        {

            lNumber *= -1;
        }
        if (lNumber < 20)
        {
            return ones[lNumber - 1];
        }
        if (lNumber <= 99)
        {
            return tens[(lNumber / 10) - 2] + NumberToWordsEN(lNumber % 10);
        }
        if (lNumber < 1000)
        {
            return NumberToWordsEN(lNumber / 100) + "Hundred " + NumberToWordsEN(lNumber % 100);
        }
        if (lNumber < 100000)
        {
            return NumberToWordsEN(lNumber / 1000) + "Thousand " + NumberToWordsEN(lNumber % 1000);
        }
        if (lNumber < 10000000)
        {
            return NumberToWordsEN(lNumber / 100000) + "Lakh " + NumberToWordsEN(lNumber % 100000);
        }
        if (lNumber < 1000000000)
        {
            return NumberToWordsEN(lNumber / 10000000) + "Crore " + NumberToWordsEN(lNumber % 10000000);
        }
        return "";
    }

    /// <summary>
    /// Chuyển số sang chữ VN
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string NumberToWordsVN(double number)
    {
        string s = number.ToString("#");
        string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
        int i, j, donvi, chuc, tram;
        string str = " ";
        bool booAm = false;
        decimal decS = 0;
        //Tung addnew
        try
        {
            decS = Convert.ToDecimal(s.ToString());
        }
        catch
        {
        }
        if (decS < 0)
        {
            decS = -decS;
            s = decS.ToString();
            booAm = true;
        }
        i = s.Length;
        if (i == 0)
        {
            str = so[0] + str;
        }
        else
        {
            j = 0;
            while (i > 0)
            {
                donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                i--;
                if (i > 0)
                {
                    chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                }
                else
                {
                    chuc = -1;
                }

                i--;
                if (i > 0)
                {
                    tram = Convert.ToInt32(s.Substring(i - 1, 1));
                }
                else
                {
                    tram = -1;
                }

                i--;
                if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                {
                    str = hang[j] + str;
                }

                j++;
                if (j > 3)
                {
                    j = 1;
                }

                if ((donvi == 1) && (chuc > 1))
                {
                    str = "một " + str;
                }
                else
                {
                    if ((donvi == 5) && (chuc > 0))
                    {
                        str = "lăm " + str;
                    }
                    else if (donvi > 0)
                    {
                        str = so[donvi] + " " + str;
                    }
                }
                if (chuc < 0)
                {
                    break;
                }
                else
                {
                    if ((chuc == 0) && (donvi > 0))
                    {
                        str = "lẻ " + str;
                    }

                    if (chuc == 1)
                    {
                        str = "mười " + str;
                    }

                    if (chuc > 1)
                    {
                        str = so[chuc] + " mươi " + str;
                    }
                }
                if (tram < 0)
                {
                    break;
                }
                else
                {
                    if ((tram > 0) || (chuc > 0) || (donvi > 0))
                    {
                        str = so[tram] + " trăm " + str;
                    }
                }
                str = " " + str;
            }
        }
        if (booAm)
        {
            str = "Âm " + str;
        }

        return str.Trim();
    }

    /// <summary>
    /// Xóa HTML tag từ chuỗi
    /// </summary>
    /// <param name="Txt"></param>
    /// <returns></returns>
    public static string StripHtml(this string Txt)
    {
        if (Txt != null)
        {
            return Regex.Replace(Txt, "<(.|\\n)*?&#>", string.Empty);
        }
        else
        {
            return "";
        }
    }
    #endregion

    #region [Object to Number]
    public static int ToInt(this object obj)
    {
        try
        {
            return int.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }
    public static float ToFloat(this object obj)
    {
        try
        {

            return float.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }
    public static decimal ToDecimal(this object obj)
    {
        try
        {
            return decimal.Parse(obj.ToString());
        }
        catch
        {
            return 0;
        }
    }
    public static double ToRound(this object obj)
    {
        try
        {
            return Math.Round(double.Parse(obj.ToString()));
        }
        catch
        {
            return 0;
        }
    }
    public static double ToDouble(this object obj)
    {
        try
        {
            return Convert.ToDouble(obj.ToString()
                .Replace(",", ";")
                .Replace(".", ",")
                .Replace(";", "."));
        }
        catch
        {
            return double.Parse(obj.ToString());
        }
    }

    #endregion

    #region [Object & JSON] 
    /// <summary>
    /// Chuyển giá trị null thành chuỗi trống hoặc 0 nếu là số
    /// </summary>
    /// <param name="obj"></param>
    public static void ClearNullable(this object obj)
    {
        Type type = obj.GetType();
        foreach (PropertyInfo pi in type.GetProperties())
        {
            object value = pi.GetValue(obj, null);
            if (value == null)
            {
                Type t = pi.PropertyType;
                switch (Type.GetTypeCode(t))
                {
                    case TypeCode.String:
                        pi.SetValue(obj, string.Empty);
                        break;

                    case TypeCode.DateTime:
                        pi.SetValue(obj, DateTime.MinValue);
                        break;

                    case TypeCode.Object:
                        pi.SetValue(obj, null);
                        break;

                    default:
                        pi.SetValue(obj, 0);
                        break;
                }
            }
        }
    }
    /// <summary>
    /// sao chép giá trị của đối tượng cùng kiểu
    /// </summary>
    /// <typeparam name="T">object source</typeparam>
    /// <typeparam name="TU">object </typeparam>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
    {
        var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
        var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

        foreach (var sourceProp in sourceProps)
        {
            if (destProps.Any(x => x.Name == sourceProp.Name))
            {
                var p = destProps.First(x => x.Name == sourceProp.Name);
                if (p.CanWrite)
                { // check if the property can be set or no.
                    Type t = p.PropertyType;
                    switch (Type.GetTypeCode(t))
                    {
                        case TypeCode.Double:
                            double zDoubleVal = sourceProp.GetValue(source, null).ToDouble();
                            p.SetValue(dest, zDoubleVal, null);
                            break;

                        case TypeCode.Int32:
                            int zIntVal = sourceProp.GetValue(source, null).ToInt();
                            p.SetValue(dest, zIntVal, null);
                            break;

                        case TypeCode.DateTime:
                            string zVal = sourceProp.GetValue(source, null).ToString();
                            DateTime zDateVal = DateTime.MinValue;

                            try
                            {
                                DateTime.TryParseExact(sourceProp.GetValue(source, null).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out zDateVal);
                            }
                            catch (Exception)
                            {
                                DateTime.TryParseExact(sourceProp.GetValue(source, null).ToString(), "dd/MM/yyyy HH:mm:tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out zDateVal);
                            }

                            p.SetValue(dest, zDateVal, null);
                            break;

                        case TypeCode.Decimal:
                            double zDecimal;
                            double.TryParse(sourceProp.GetValue(source, null).ToString(), out zDecimal);
                            p.SetValue(dest, zDecimal, null);
                            break;

                        default:
                            p.SetValue(dest, sourceProp.GetValue(source, null).ToString(), null);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Kiểm tra từng giá trị trong 2 đối tượng cùng kiểu dữ liệu
    /// </summary>
    /// <param name="obj">obj 1</param>
    /// <param name="another">obj 2 </param>
    /// <returns>true, false</returns>
    public static bool CompareObject(object obj, object another)
    {
        if (ReferenceEquals(obj, another))
        {
            return true;
        }

        if ((obj == null) || (another == null))
        {
            return false;
        }
        //Compare two object's class, return false if they are difference
        if (obj.GetType() != another.GetType())
        {
            return false;
        }

        var result = true;
        //Get all properties of obj
        //And compare each other
        foreach (var property in obj.GetType().GetProperties())
        {
            var objValue = property.GetValue(obj);
            var anotherValue = property.GetValue(another);
            if (!objValue.Equals(anotherValue))
            {
                result = false;
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Kiểm tra từng giá trị trong 2 đối tượng cùng kiểu dữ liệu
    /// </summary>
    /// <param name="obj">This OBJECT</param>
    /// <param name="another">New OBJECT</param>
    /// <returns>New Object</returns>
    public static object CopyObject(this object obj, object another, out string Message)
    {
        //Compare two object's class, return false if they are difference
        if (obj.GetType() != another.GetType())
        {
            Message = "2 object must be the same";
        }

        //Get all properties of obj
        //And compare each other
        foreach (var property in obj.GetType().GetProperties())
        {
            var objValue = property.GetValue(obj);
            var anotherValue = property.GetValue(another);
            if (objValue != null &&
                objValue.ToString() != string.Empty)
            {
                anotherValue = objValue;
                //result = false;
                //break;
            }
        }
        Message = "OK";
        return another;
    }

    #endregion

    #region [Chức năng khác]
    public static string FormatMoney(this object input)
    {
        try
        {
            if (input.ToString() != "0")
            {
                double zResult = double.Parse(input.ToString());

                string n = "";
                if (zResult % 1 == 0)
                {
                    n = "n0";
                }
                else
                {
                    n = "n2";
                }

                return zResult.ToString(n, CultureInfo.GetCultureInfo("vi-VN"));
            }
            else
            {
                return "0";
            }
        }
        catch (Exception)
        {
            return "Lỗi số";
        }
    }

    /// <summary>
    /// parse datetime from object to dd/MM/yyyy
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToDateString(this object obj)
    {
        try
        {
            string s = "";
            DateTime zDate = DateTime.Parse(obj.ToString());
            if (zDate != DateTime.MinValue)
            {
                s = zDate.ToString("dd/MM/yyyy");
            }

            return s;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static bool ToBool(this object obj)
    {
        try
        {
            return bool.Parse(obj.ToString());
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static string ToAscii(this string unicode)
    {

        unicode = unicode.ToLower().Trim();
        unicode = Regex.Replace(unicode, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
        unicode = Regex.Replace(unicode, "[๖ۣۜ]", "");
        unicode = Regex.Replace(unicode, "[óòỏõọôồốổỗộơớờởỡợ]", "o");
        unicode = Regex.Replace(unicode, "[éèẻẽẹêếềểễệ]", "e");
        unicode = Regex.Replace(unicode, "[íìỉĩị]", "i");
        unicode = Regex.Replace(unicode, "[úùủũụưứừửữự]", "u");
        unicode = Regex.Replace(unicode, "[ýỳỷỹỵ]", "y");
        unicode = Regex.Replace(unicode, "[đ]", "d");
        unicode = unicode.Replace(" ", "-").Replace("[()]", "");
        unicode = Regex.Replace(unicode, "[-\\s+/]+", "-");
        unicode = Regex.Replace(unicode, "\\W+", "-"); //Nếu bạn muốn thay dấu khoảng trắng thành dấu "_" hoặc dấu cách " " thì thay kí tự bạn muốn vào đấu "-"
        return unicode;
    }
    public static string ToEnglish(string s)
    {
        string sspace = s.Replace(" ", "");
        string slow = sspace.ToLower();
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        string temp = slow.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }

    public static string UrlEncode(this string input)
    {
        return System.Web.HttpUtility.UrlEncode(input);
    }
    public static string UrlDecode(this string input)
    {
        return System.Web.HttpUtility.UrlDecode(input);
    }

    /// <summary>
    /// Tính lấy năm
    /// </summary>
    /// <param name="FromDate"></param>
    /// <param name="ToDate"></param>
    /// <returns></returns>
    public static int GetTotalYear(DateTime FromDate, DateTime ToDate)
    {
        DateTime zeroTime = new DateTime(1, 1, 1);

        DateTime a = FromDate;
        DateTime b = ToDate;

        TimeSpan span = b - a;
        // Because we start at year 1 for the Gregorian
        // calendar, we must subtract a year here.
        int years = (zeroTime + span).Year - 1;

        // 1, where my other algorithm resulted in 0.
        return years;
    }

    /// <summary>
    /// ước lượng thời gian như facebook
    /// </summary>
    /// <param name="FromDate"></param>
    /// <param name="ToDate"></param>
    /// <returns></returns>
    public static string TimeAgo(DateTime FromDate, DateTime ToDate)
    {
        TimeSpan span = ToDate - FromDate;

        if (span.Days > 365)
        {
            int years = (span.Days / 365);
            if (span.Days % 365 != 0)
            {
                years += 1;
            }

            //return String.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
            return String.Format("khoản {0} năm trước", years);
        }

        if (span.Days > 30)
        {
            int months = (span.Days / 30);
            if (span.Days % 31 != 0)
            {
                months += 1;
            }

            //return String.Format("about {0} {1} ago", months, months == 1 ? "month" : "months");
            return String.Format("khoản {0} tháng trước", months);
        }

        if (span.Days > 0)
        {
            //return String.Format("about {0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");
            return String.Format("khoản {0} ngày trước", span.Days);
        }

        if (span.Hours > 0)
        {
            //return String.Format("about {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");
            return String.Format("khoản {0} giờ trước", span.Hours);
        }

        if (span.Minutes > 0)
        {
            //return String.Format("about {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            return String.Format("khoản {0} phút trước", span.Minutes);
        }

        if (span.Seconds > 5)
        {
            //return String.Format("about {0} seconds ago", span.Seconds);
            return String.Format("khoản {0} vài giây trước", span.Seconds);
        }

        if (span.Seconds <= 5)
        {
            return "mới gần đây !.";
        }

        return string.Empty;
    }
    public static string CalculateYourTime(DateTime FromDate, DateTime ToDate, out int YearNum)
    {

        int Years = new DateTime(ToDate.Subtract(FromDate).Ticks).Year - 1;
        DateTime _DOBDateNow = FromDate.AddYears(Years);
        int Months = 0;
        for (int i = 1; i <= 12; i++)
        {
            if (_DOBDateNow.AddMonths(i) == ToDate)
            {
                Months = i;
                break;
            }
            else if (_DOBDateNow.AddMonths(i) >= ToDate)
            {
                Months = i - 1;
                break;
            }
        }
        YearNum = Years;
        int Days = ToDate.Subtract(_DOBDateNow.AddMonths(Months)).Days;
        return "" + Years + " năm, " + Months + " tháng, " + Days + " ngày.";
        //return $"Age is {_Years} Years {_Months} Months {Days} Days";
    }

    /// <summary>
    /// Lấy chỉ số tiền USD mua bán của VIETCOMBANK
    /// </summary>
    /// <param name="CurrencyCode">USD</param>
    /// <returns></returns>
    public static string USDRate_VietComBank(string CurrencyCode)
    {
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            String xmlSourceUrl = "http://www.vietcombank.com.vn/ExchangeRates/ExrateXML.aspx";
            xmlDocument.Load(xmlSourceUrl);

            //từ đây hoàn toàn có thể thao tác dữ liệu xml bằng đối tượng xmlDocument
            //lấy ví zụ chuyển từ XmlDocument thành tập các đối tượng Generic dạng List<Exrate>
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Exrate");
            List<Exrate> listExrate = null;
            if (nodeList != null && nodeList.Count > 0)
            {
                listExrate = new List<Exrate>();
                foreach (XmlNode xmlNode in nodeList)
                {
                    Exrate entityExrate = new Exrate();
                    entityExrate.CurrencyCode = xmlNode.Attributes["CurrencyCode"].InnerText;
                    entityExrate.CurrencyName = xmlNode.Attributes["CurrencyName"].InnerText;
                    entityExrate.Transfer = float.Parse(xmlNode.Attributes["Transfer"].InnerText);
                    entityExrate.Buy = float.Parse(xmlNode.Attributes["Buy"].InnerText);   //tỷ giá mua vào
                    entityExrate.Sell = float.Parse(xmlNode.Attributes["Sell"].InnerText);  //tỷ giá bán ra

                    listExrate.Add(entityExrate);
                }
            }

            return listExrate.Single(s => s.CurrencyCode == CurrencyCode).Sell.ToString();
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }


    /// <summary>
    /// Kiểm tra file đang sử dụng hoặc không
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }

    /// <summary>
    /// Tạo mã MD5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public static string HashPass(string input)
    {
        HashAlgorithm Hash = HashAlgorithm.Create("SHA1");
        byte[] pwordData = Encoding.Default.GetBytes(input);
        byte[] bHash = Hash.ComputeHash(pwordData);
        return Convert.ToBase64String(bHash);
    }

    public static Boolean VerifyHash(string NewPass, string OldPass)
    {
        string HashNewPass = HashPass(NewPass);
        return (OldPass == HashNewPass);
    }
    #endregion
}

//vietcombank format class
public class Exrate
{
    private string _CurrencyCode = string.Empty;

    public string CurrencyCode
    {
        get { return _CurrencyCode; }
        set { _CurrencyCode = value; }
    }
    private string _CurrencyName = string.Empty;

    public string CurrencyName
    {
        get { return _CurrencyName; }
        set { _CurrencyName = value; }
    }
    private float _Buy = 0;

    public float Buy
    {
        get { return _Buy; }
        set { _Buy = value; }
    }
    private float _Transfer = 0;

    public float Transfer
    {
        get { return _Transfer; }
        set { _Transfer = value; }
    }
    private float _Sell = 0;

    public float Sell
    {
        get { return _Sell; }
        set { _Sell = value; }
    }
}


/// <summary>
/// ServerResult kết quả trả về của server
/// </summary>
public class ServerResult
{
    public string Message
    {
        get;
        set;
    } = "";
    public bool Success
    {
        get;
        set;
    } = false;
    public int Code
    {
        get;
        set;
    } = 0;

    public string Data
    {
        get;
        set;
    } = "";

    public ServerResult()
    {

    }
}