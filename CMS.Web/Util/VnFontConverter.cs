namespace CMS.Web.Util
{
    public class VnFontConverter
    {
        #region -- PRIVATE MEMBERS

        private string[,] Code;
        private FontCase m_CharCase = FontCase.Normal;

        #endregion

        #region -- CONSTRUCTORS --

        public VnFontConverter()
        {
            InitData();
        }

        #endregion

        #region -- PROPERTIES --

        public FontCase CharCase
        {
            get { return m_CharCase; }
            set { m_CharCase = value; }
        }

        #endregion

        #region -- Methods --

        public static string ABCToUnicocde(string input)
        {
            if (input != string.Empty || input != null)
            {
                VnFontConverter convert = new VnFontConverter();
                convert.Convert(ref input, FontIndex.iTCV, FontIndex.iUNI);
            }
            else
            {
                return string.Empty;
            }
            return input;
        }

        public bool Convert(ref string strConv, FontIndex iSource, FontIndex iDestination)
        {
            // Convert từ kiểu enum sang int tương ứng
            int sourceIndex = (int)iSource;
            int destinationIndex = (int)iDestination;

            // Trim đầu và cuối
            if (strConv.Trim() == "")
            {
                return false;
            }

            // Kiểm tra nếu bảng nguồn = bảng đích
            if (sourceIndex == destinationIndex)
            {
                return false;
            }
            string str = "";
            string str2 = "";

            // Nếu bảng nguồn chưa biết thì dùng hàm check
            if (sourceIndex == -1)
            {
                int tempIndex = (int)getCode(strConv);
                if (tempIndex <= -1)
                {
                    return false;
                }
                sourceIndex = tempIndex;
            }

            // Nếu bảng đích = chưa biết thì bảng đích = 0
            if (destinationIndex == -1)
            {
                destinationIndex = 0;
            }

            int nChar = GetnChar((FontIndex)int.Parse(Code[0, sourceIndex]));

            int num5;
            if (nChar > 1)
            {
                num5 = nChar - 1;
            }
            else
            {
                num5 = 1;
            }

            string str3 = "";

            bool flag = false;

            strConv = strConv + "       ";

            int startIndex = 0;

            while (startIndex < (strConv.Length - 7))
            {
                for (int i = nChar; i >= num5; i--)
                {
                    str2 = "";
                    if (strConv.Substring(startIndex, 1) == " ")
                    {
                        str = " ";
                        break;
                    }

                    str = strConv.Substring(startIndex, i);
                    for (int j = 1; j < 0x87; j++)
                    {
                        if (str == Code[j, sourceIndex])
                        {
                            if ((m_CharCase == FontCase.UpperCase) && (j < 0x44))
                            {
                                str2 = Code[j + 0x43, destinationIndex];
                            }
                            else if ((m_CharCase == FontCase.LowerCase) && (j >= 0x44))
                            {
                                str2 = Code[j - 0x43, destinationIndex];
                            }
                            else
                            {
                                str2 = Code[j, destinationIndex];
                            }
                            startIndex += i;
                            break;
                        }
                    }

                    if ((str2 != "") || (i == 5))
                    {
                        break;
                    }
                }
                if (str2 != "")
                {
                    str3 = str3 + str2;
                    flag = true;
                }
                else
                {
                    if (m_CharCase == FontCase.UpperCase)
                    {
                        str3 = str3 + str.Substring(0, 1).ToUpper();
                    }
                    else if (m_CharCase == FontCase.LowerCase)
                    {
                        str3 = str3 + str.Substring(0, 1).ToLower();
                    }
                    else
                    {
                        str3 = str3 + str.Substring(0, 1);
                    }
                    startIndex++;
                }
            }
            if (!flag)
            {
                strConv = strConv.Remove(strConv.Length - 7, 7);
                if (m_CharCase == FontCase.UpperCase)
                {
                    strConv = strConv.ToUpper();
                    return true;
                }
                if (m_CharCase == FontCase.LowerCase)
                {
                    strConv = strConv.ToLower();
                    return true;
                }
                return false;
            }
            strConv = str3.TrimEnd(new char[0]);
            return true;
        }

        public bool Convert2(ref string strConv, FontIndex iSource, FontIndex iDestination)
        {
            // Convert từ kiểu enum sang int tương ứng
            int sourceIndex = (int)iSource;
            int destinationIndex = (int)iDestination;

            // Trim đầu và cuối
            if (strConv.Trim() == "")
            {
                return false;
            }

            // Kiểm tra nếu bảng nguồn = bảng đích
            if (sourceIndex == destinationIndex)
            {
                return false;
            }

            // Nếu bảng nguồn chưa biết thì dùng hàm check
            if (sourceIndex == -1)
            {
                int tempIndex = (int)getCode(strConv);
                if (tempIndex <= -1)
                {
                    return false;
                }
                sourceIndex = tempIndex;
            }

            // Nếu bảng đích = chưa biết thì bảng đích = 0
            if (destinationIndex == -1)
            {
                destinationIndex = 0;
            }

            for (int j = 0; j <= 134; j++)
            {
                strConv = strConv.Replace(Code[j, sourceIndex], Code[j, destinationIndex]);
            }
            return true;
        }

        public static string Convert(string strConv, FontIndex iSource, FontIndex iDestination)
        {
            if (strConv != string.Empty || strConv != null)
            {
                VnFontConverter convert = new VnFontConverter();
                convert.Convert2(ref strConv, iSource, iDestination);
            }
            else
            {
                return string.Empty;
            }
            return strConv;
        }

        private FontIndex getCode(string str)
        {
            if (str.Trim() == "")
            {
                return FontIndex.iNotKnown;
            }
            int num = -1;
            str = str + "       ";
            int startIndex = 0;
            while (startIndex < (str.Length - 7))
            {
                if (str.Substring(startIndex, 1) == " ")
                {
                    startIndex++;
                }
                else
                {
                    for (int i = 7; i > 0; i--)
                    {
                        string s = str.Substring(startIndex, i);
                        for (int j = 0; j < 7; j++)
                        {
                            if (((i == 4) || (i == 5)) || ((i >= 6) && (j != 0)))
                            {
                                break;
                            }
                            if (((i != 3) || (j == 1)) &&
                                ((((j != 3) && (j != 2)) && ((j != 5) && (j != 4))) || (i <= 2)))
                            {
                                for (int k = 1; k < 0x87; k++)
                                {
                                    if (s == Code[k, j])
                                    {
                                        if (!isSpecialChar(s, (j == 5) || (j == 6)))
                                        {
                                            return (FontIndex)int.Parse(Code[0, j]);
                                        }
                                        num = j;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    startIndex++;
                }
            }
            return ((num >= 0) ? ((FontIndex)int.Parse(Code[0, num])) : FontIndex.iNotKnown);
        }


        private static int GetnChar(FontIndex index)
        {
            if (index == FontIndex.iUTF)
            {
                return 3;
            }
            if (((index == FontIndex.iUNI) || (index == FontIndex.iUTH)) || (index == FontIndex.iNOSIGN))
            {
                return 1;
            }
            if (index == FontIndex.iNCR)
            {
                return 7;
            }
            return 2;
        }

        private void InitData()
        {
            Code = new string[0x87, 21];
            Code[0, 0] = 0.ToString();
            Code[0, 1] = 1.ToString();
            Code[0, 2] = 2.ToString();
            Code[0, 3] = 3.ToString();
            Code[0, 5] = 5.ToString();
            Code[0, 6] = 6.ToString();
            Code[0, 4] = 4.ToString();
            Code[0, 7] = 7.ToString();
            Code[0, 20] = 20.ToString();
            MapUnicode();
            MapVNI();
            MapTCV();
            MapUTH();
            MapUTF8();
            MapNCR();
            MapNoSign();
            MapCP1258();
            MapHTML();
        }


        private static bool isSpecialChar(string s, bool isUNI)
        {
            if (s.Length <= 2)
            {
                string[] strArray3;
                string[] strArray = new string[]
                                        {
                                            "\x00ed", "\x00ec", "\x00f3", "\x00f2", "\x00f4", "\x00f1", "\x00ee",
                                            "\x00ca", "\x00c8",
                                            "\x00c9", "\x00e1", "\x00e0", "\x00e2", "\x00e8", "\x00e9", "\x00ea",
                                            "\x00f9", "\x00fd", "\x00fa", "\x00f6", "\x00cd", "\x00cc", "\x00d3",
                                            "\x00d2", "\x00d4",
                                            "\x00d1", "\x00ce", "\x00d5", "\x00dd", "\x00c3", "o\x00e0", "o\x00e1",
                                            "o\x00e3", "u\x00fb", "O\x00c1", "O\x00c0", "O\x00c3"
                                        };
                string[] strArray2 = new string[]
                                         {
                                             "a", "\x00e2", "\x00ea", "\x00f4", "o", "u", "d", "\x00ed", "\x00ec",
                                             "\x00f3", "\x00f2",
                                             "\x00f4", "\x00f1", "\x00ee", "\x00ca", "\x00c8",
                                             "\x00c9", "\x00e1", "\x00e0", "\x00e2", "\x00e8", "\x00e9", "\x00ea",
                                             "\x00f9", "\x00fd",
                                             "\x00fa", "\x00f6", "\x00cd", "\x00cc", "\x00d3", "\x00d2", "\x00d4",
                                             "\x00d1", "\x00ce", "\x00d5", "\x00dd", "\x00c3", "o\x00e0", "o\x00e1",
                                             "o\x00e3", "u\x00fb",
                                             "O\x00c1", "O\x00c0", "O\x00c3"
                                         };
                if (!isUNI)
                {
                    strArray3 = strArray;
                }
                else
                {
                    strArray3 = strArray2;
                }
                for (int i = 0; i < strArray3.Length; i++)
                {
                    if (string.Compare(s, strArray3[i], true) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isVietnamese(string s)
        {
            return (getCode(s) != FontIndex.iNotKnown);
        }

        public bool isVietnamese(string s, ref FontIndex code)
        {
            code = getCode(s);
            return (code > FontIndex.iNotKnown);
        }

        #endregion

        #region -- CHARACTER MAP --

        private void MapBKHCM1()
        {
            Code[1, 6] = "\x00be";
            Code[2, 6] = "\x00bf";
            Code[3, 6] = "\x00c0";
            Code[4, 6] = "\x00c1";
            Code[5, 6] = "\x00c2";
            Code[6, 6] = "\x00d7";
            Code[7, 6] = "\x00d8";
            Code[8, 6] = "\x00d9";
            Code[9, 6] = "\x00da";
            Code[10, 6] = "\x00db";
            Code[11, 6] = "\x00dc";
            Code[12, 6] = "\x00dd";
            Code[13, 6] = "\x00de";
            Code[14, 6] = "\x00df";
            Code[15, 6] = "\x00e0";
            Code[0x10, 6] = "\x00e1";
            Code[0x11, 6] = "\x00e2";
            Code[0x12, 6] = "\x00c3";
            Code[0x13, 6] = "\x00c4";
            Code[20, 6] = "\x00c5";
            Code[0x15, 6] = "\x00c6";
            Code[0x16, 6] = "\x00c7";
            Code[0x17, 6] = "\x00e3";
            Code[0x18, 6] = "\x00e4";
            Code[0x19, 6] = "\x00e5";
            Code[0x1a, 6] = "\x00e6";
            Code[0x1b, 6] = "\x00e7";
            Code[0x1c, 6] = "\x00e8";
            Code[0x1d, 6] = "\x00c8";
            Code[30, 6] = "\x00c9";
            Code[0x1f, 6] = "\x00ca";
            Code[0x20, 6] = "\x00cb";
            Code[0x21, 6] = "\x00cc";
            Code[0x22, 6] = "\x00cd";
            Code[0x23, 6] = "\x00ce";
            Code[0x24, 6] = "\x00cf";
            Code[0x25, 6] = "\x00d0";
            Code[0x26, 6] = "\x00d1";
            Code[0x27, 6] = "\x00e9";
            Code[40, 6] = "\x00ea";
            Code[0x29, 6] = "\x00eb";
            Code[0x2a, 6] = "\x00ec";
            Code[0x2b, 6] = "\x00ed";
            Code[0x2c, 6] = "\x00ee";
            Code[0x2d, 6] = "\x00ef";
            Code[0x2e, 6] = "\x00f0";
            Code[0x2f, 6] = "\x00f1";
            Code[0x30, 6] = "\x00f2";
            Code[0x31, 6] = "\x00f3";
            Code[50, 6] = "\x00f4";
            Code[0x33, 6] = "\x00d2";
            Code[0x34, 6] = "\x00d3";
            Code[0x35, 6] = "\x00d4";
            Code[0x36, 6] = "\x00d5";
            Code[0x37, 6] = "\x00d6";
            Code[0x38, 6] = "\x00f5";
            Code[0x39, 6] = "\x00f6";
            Code[0x3a, 6] = "\x00f7";
            Code[0x3b, 6] = "\x00f8";
            Code[60, 6] = "\x00f9";
            Code[0x3d, 6] = "\x00fa";
            Code[0x3e, 6] = "\x00fb";
            Code[0x3f, 6] = "\x00fc";
            Code[0x40, 6] = "\x00fd";
            Code[0x41, 6] = "\x00fe";
            Code[0x42, 6] = "\x00ff";
            Code[0x43, 6] = "\x00bd";
            Code[0x44, 6] = "€";
            Code[0x45, 6] = "\x0081";
            Code[70, 6] = "‚";
            Code[0x47, 6] = "ƒ";
            Code[0x48, 6] = "„";
            Code[0x49, 6] = "™";
            Code[0x4a, 6] = "š";
            Code[0x4b, 6] = "›";
            Code[0x4c, 6] = "œ";
            Code[0x4d, 6] = "\x009d";
            Code[0x4e, 6] = "˜";
            Code[0x4f, 6] = "Ÿ";
            Code[80, 6] = "~";
            Code[0x51, 6] = "\x00a1";
            Code[0x52, 6] = "\x00a2";
            Code[0x53, 6] = "\x00a3";
            Code[0x54, 6] = "\x00a4";
            Code[0x55, 6] = "…";
            Code[0x56, 6] = "†";
            Code[0x57, 6] = "‡";
            Code[0x58, 6] = "ˆ";
            Code[0x59, 6] = "‰";
            Code[90, 6] = "\x00a5";
            Code[0x5b, 6] = "\x00a6";
            Code[0x5c, 6] = "\x00a7";
            Code[0x5d, 6] = "\x00a8";
            Code[0x5e, 6] = "\x00a9";
            Code[0x5f, 6] = "\x00aa";
            Code[0x60, 6] = "Š";
            Code[0x61, 6] = "‹";
            Code[0x62, 6] = "Œ";
            Code[0x63, 6] = "\x008d";
            Code[100, 6] = "Ž";
            Code[0x65, 6] = "\x008f";
            Code[0x66, 6] = "\x0090";
            Code[0x67, 6] = "‘";
            Code[0x68, 6] = "’";
            Code[0x69, 6] = "“";
            Code[0x6a, 6] = "\x00ab";
            Code[0x6b, 6] = "\x00ac";
            Code[0x6c, 6] = "\x00ad";
            Code[0x6d, 6] = "\x00ae";
            Code[110, 6] = "\x00af";
            Code[0x6f, 6] = "\x00b0";
            Code[0x70, 6] = "\x00b1";
            Code[0x71, 6] = "\x00b2";
            Code[0x72, 6] = "\x00b3";
            Code[0x73, 6] = "\x00b4";
            Code[0x74, 6] = "\x00b5";
            Code[0x75, 6] = "\x00b6";
            Code[0x76, 6] = "”";
            Code[0x77, 6] = "•";
            Code[120, 6] = "–";
            Code[0x79, 6] = "—";
            Code[0x7a, 6] = "˜";
            Code[0x7b, 6] = "\x00b7";
            Code[0x7c, 6] = "\x00b8";
            Code[0x7d, 6] = "\x00b9";
            Code[0x7e, 6] = "\x00ba";
            Code[0x7f, 6] = "\x00bb";
            Code[0x80, 6] = "\x00bc";
            Code[0x81, 6] = "{";
            Code[130, 6] = "^";
            Code[0x83, 6] = "`";
            Code[0x84, 6] = "|";
            Code[0x85, 6] = "Ž";
            Code[0x86, 6] = "}";
        }

        private void MapBKHCM2()
        {
            Code[1, 6] = "a\x00e1";
            Code[2, 6] = "a\x00e2";
            Code[3, 6] = "a\x00e3";
            Code[4, 6] = "a\x00e4";
            Code[5, 6] = "a\x00e5";
            Code[6, 6] = "\x00f9";
            Code[7, 6] = "\x00f9\x00e6";
            Code[8, 6] = "\x00f9\x00e7";
            Code[9, 6] = "\x00f9\x00e8";
            Code[10, 6] = "\x00f9\x00e9";
            Code[11, 6] = "\x00f9\x00e5";
            Code[12, 6] = "\x00ea";
            Code[13, 6] = "\x00ea\x00eb";
            Code[14, 6] = "\x00ea\x00ec";
            Code[15, 6] = "\x00ea\x00ed";
            Code[0x10, 6] = "\x00ea\x00ee";
            Code[0x11, 6] = "\x00ea\x00e5";
            Code[0x12, 6] = "e\x00e1";
            Code[0x13, 6] = "e\x00e2";
            Code[20, 6] = "e\x00e3";
            Code[0x15, 6] = "e\x00e4";
            Code[0x16, 6] = "e\x00e5";
            Code[0x17, 6] = "\x00ef";
            Code[0x18, 6] = "\x00ef\x00eb";
            Code[0x19, 6] = "\x00ef\x00ec";
            Code[0x1a, 6] = "\x00ef\x00ed";
            Code[0x1b, 6] = "\x00ef\x00ee";
            Code[0x1c, 6] = "\x00ef\x00e5";
            Code[0x1d, 6] = "\x00f1";
            Code[30, 6] = "\x00f2";
            Code[0x1f, 6] = "\x00f3";
            Code[0x20, 6] = "\x00f4";
            Code[0x21, 6] = "\x00f5";
            Code[0x22, 6] = "o\x00e1";
            Code[0x23, 6] = "o\x00e2";
            Code[0x24, 6] = "o\x00e3";
            Code[0x25, 6] = "o\x00e4";
            Code[0x26, 6] = "o\x00e5";
            Code[0x27, 6] = "\x00f6";
            Code[40, 6] = "\x00f6\x00eb";
            Code[0x29, 6] = "\x00f6\x00ec";
            Code[0x2a, 6] = "\x00f6\x00ed";
            Code[0x2b, 6] = "\x00f6\x00ee";
            Code[0x2c, 6] = "\x00f6\x00e5";
            Code[0x2d, 6] = "\x00fa";
            Code[0x2e, 6] = "\x00fa\x00e1";
            Code[0x2f, 6] = "\x00fa\x00e2";
            Code[0x30, 6] = "\x00fa\x00e3";
            Code[0x31, 6] = "\x00fa\x00e4";
            Code[50, 6] = "\x00fa\x00e5";
            Code[0x33, 6] = "u\x00e1";
            Code[0x34, 6] = "u\x00e2";
            Code[0x35, 6] = "u\x00e3";
            Code[0x36, 6] = "u\x00e4";
            Code[0x37, 6] = "u\x00e5";
            Code[0x38, 6] = "\x00fb";
            Code[0x39, 6] = "\x00fb\x00e1";
            Code[0x3a, 6] = "\x00fb\x00e2";
            Code[0x3b, 6] = "\x00fb\x00e3";
            Code[60, 6] = "\x00fb\x00e4";
            Code[0x3d, 6] = "\x00fb\x00e5";
            Code[0x3e, 6] = "y\x00e1";
            Code[0x3f, 6] = "y\x00e2";
            Code[0x40, 6] = "y\x00e3";
            Code[0x41, 6] = "y\x00e4";
            Code[0x42, 6] = "y\x00e5";
            Code[0x43, 6] = "\x00e0";
            Code[0x44, 6] = "A\x00c1";
            Code[0x45, 6] = "A\x00c2";
            Code[70, 6] = "A\x00c3";
            Code[0x47, 6] = "A\x00c4";
            Code[0x48, 6] = "A\x00c5";
            Code[0x49, 6] = "\x00d9";
            Code[0x4a, 6] = "\x00d9\x00c6";
            Code[0x4b, 6] = "\x00d9\x00c7";
            Code[0x4c, 6] = "\x00d9\x00c8";
            Code[0x4d, 6] = "\x00d9\x00c9";
            Code[0x4e, 6] = "\x00d9\x00c5";
            Code[0x4f, 6] = "\x00ca";
            Code[80, 6] = "\x00ca\x00cb";
            Code[0x51, 6] = "\x00ca\x00cc";
            Code[0x52, 6] = "\x00ca\x00cd";
            Code[0x53, 6] = "\x00ca\x00ce";
            Code[0x54, 6] = "\x00ca\x00c5";
            Code[0x55, 6] = "E\x00c1";
            Code[0x56, 6] = "E\x00c2";
            Code[0x57, 6] = "E\x00c3";
            Code[0x58, 6] = "E\x00c4";
            Code[0x59, 6] = "E\x00c5";
            Code[90, 6] = "\x00cf";
            Code[0x5b, 6] = "\x00cf\x00cb";
            Code[0x5c, 6] = "\x00cf\x00cc";
            Code[0x5d, 6] = "\x00cf\x00cd";
            Code[0x5e, 6] = "\x00cf\x00ce";
            Code[0x5f, 6] = "\x00cf\x00e5";
            Code[0x60, 6] = "\x00d1";
            Code[0x61, 6] = "\x00d2";
            Code[0x62, 6] = "\x00d3";
            Code[0x63, 6] = "\x00d4";
            Code[100, 6] = "\x00d5";
            Code[0x65, 6] = "O\x00c1";
            Code[0x66, 6] = "O\x00c2";
            Code[0x67, 6] = "O\x00c3";
            Code[0x68, 6] = "O\x00c4";
            Code[0x69, 6] = "O\x00c5";
            Code[0x6a, 6] = "\x00d6";
            Code[0x6b, 6] = "\x00d6\x00cb";
            Code[0x6c, 6] = "\x00d6\x00cc";
            Code[0x6d, 6] = "\x00d6\x00cd";
            Code[110, 6] = "\x00d6\x00ce";
            Code[0x6f, 6] = "\x00d6\x00c5";
            Code[0x70, 6] = "\x00da";
            Code[0x71, 6] = "\x00da\x00c1";
            Code[0x72, 6] = "\x00da\x00c2";
            Code[0x73, 6] = "\x00da\x00c3";
            Code[0x74, 6] = "\x00da\x00c4";
            Code[0x75, 6] = "\x00da\x00c5";
            Code[0x76, 6] = "U\x00c1";
            Code[0x77, 6] = "U\x00c2";
            Code[120, 6] = "U\x00c3";
            Code[0x79, 6] = "U\x00c4";
            Code[0x7a, 6] = "U\x00c5";
            Code[0x7b, 6] = "\x00db";
            Code[0x7c, 6] = "\x00db\x00c1";
            Code[0x7d, 6] = "\x00db\x00c2";
            Code[0x7e, 6] = "\x00db\x00c3";
            Code[0x7f, 6] = "\x00db\x00c4";
            Code[0x80, 6] = "\x00db\x00c5";
            Code[0x81, 6] = "Y\x00c1";
            Code[130, 6] = "Y\x00c2";
            Code[0x83, 6] = "Y\x00c3";
            Code[0x84, 6] = "Y\x00c4";
            Code[0x85, 6] = "Y\x00c5";
            Code[0x86, 6] = "\x00c0";
        }

        private void MapCP1258()
        {
            Code[1, 4] = "a\x00ec";
            Code[2, 4] = "a\x00cc";
            Code[3, 4] = "a\x00d2";
            Code[4, 4] = "a\x00de";
            Code[5, 4] = "a\x00f2";
            Code[6, 4] = "\x00e3";
            Code[7, 4] = "\x00e3\x00ec";
            Code[8, 4] = "\x00e3\x00cc";
            Code[9, 4] = "\x00e3\x00d2";
            Code[10, 4] = "\x00e3\x00de";
            Code[11, 4] = "\x00e3\x00f2";
            Code[12, 4] = "\x00e2";
            Code[13, 4] = "\x00e2\x00ec";
            Code[14, 4] = "\x00e2\x00cc";
            Code[15, 4] = "\x00e2\x00d2";
            Code[0x10, 4] = "\x00e2\x00de";
            Code[0x11, 4] = "\x00e2\x00f2";
            Code[0x12, 4] = "e\x00ec";
            Code[0x13, 4] = "e\x00cc";
            Code[20, 4] = "e\x00d2";
            Code[0x15, 4] = "e\x00de";
            Code[0x16, 4] = "e\x00f2";
            Code[0x17, 4] = "\x00ea";
            Code[0x18, 4] = "\x00ea\x00ec";
            Code[0x19, 4] = "\x00ea\x00cc";
            Code[0x1a, 4] = "\x00ea\x00d2";
            Code[0x1b, 4] = "\x00ea\x00de";
            Code[0x1c, 4] = "\x00ea\x00f2";
            Code[0x1d, 4] = "i\x00ec";
            Code[30, 4] = "i\x00cc";
            Code[0x1f, 4] = "i\x00d2";
            Code[0x20, 4] = "i\x00de";
            Code[0x21, 4] = "i\x00f2";
            Code[0x22, 4] = "o\x00ec";
            Code[0x23, 4] = "o\x00cc";
            Code[0x24, 4] = "o\x00d2";
            Code[0x25, 4] = "o\x00de";
            Code[0x26, 4] = "o\x00f2";
            Code[0x27, 4] = "\x00f4";
            Code[40, 4] = "\x00f4\x00ec";
            Code[0x29, 4] = "\x00f4\x00cc";
            Code[0x2a, 4] = "\x00f4\x00d2";
            Code[0x2b, 4] = "\x00f4\x00de";
            Code[0x2c, 4] = "\x00f4\x00f2";
            Code[0x2d, 4] = "\x00f5";
            Code[0x2e, 4] = "\x00f5\x00ec";
            Code[0x2f, 4] = "\x00f5\x00cc";
            Code[0x30, 4] = "\x00f5\x00d2";
            Code[0x31, 4] = "\x00f5\x00de";
            Code[50, 4] = "\x00f5\x00f2";
            Code[0x33, 4] = "u\x00ec";
            Code[0x34, 4] = "u\x00cc";
            Code[0x35, 4] = "u\x00d2";
            Code[0x36, 4] = "u\x00de";
            Code[0x37, 4] = "u\x00f2";
            Code[0x38, 4] = "\x00fd";
            Code[0x39, 4] = "\x00fd\x00ec";
            Code[0x3a, 4] = "\x00fd\x00cc";
            Code[0x3b, 4] = "\x00fd\x00d2";
            Code[60, 4] = "\x00fd\x00de";
            Code[0x3d, 4] = "\x00fd\x00f2";
            Code[0x3e, 4] = "y\x00ec";
            Code[0x3f, 4] = "y\x00cc";
            Code[0x40, 4] = "y\x00d2";
            Code[0x41, 4] = "y\x00de";
            Code[0x42, 4] = "y\x00f2";
            Code[0x43, 4] = "\x00f0";
            Code[0x44, 4] = "A\x00ec";
            Code[0x45, 4] = "A\x00cc";
            Code[70, 4] = "A\x00d2";
            Code[0x47, 4] = "A\x00de";
            Code[0x48, 4] = "A\x00f2";
            Code[0x49, 4] = "\x00c3";
            Code[0x4a, 4] = "\x00c3\x00ec";
            Code[0x4b, 4] = "\x00c3\x00cc";
            Code[0x4c, 4] = "\x00c3\x00d2";
            Code[0x4d, 4] = "\x00c3\x00de";
            Code[0x4e, 4] = "\x00c3\x00f2";
            Code[0x4f, 4] = "\x00c2";
            Code[80, 4] = "\x00c2\x00ec";
            Code[0x51, 4] = "\x00c2\x00cc";
            Code[0x52, 4] = "\x00c2\x00d2";
            Code[0x53, 4] = "\x00c2\x00de";
            Code[0x54, 4] = "\x00c2\x00f2";
            Code[0x55, 4] = "E\x00ec";
            Code[0x56, 4] = "E\x00cc";
            Code[0x57, 4] = "E\x00d2";
            Code[0x58, 4] = "E\x00de";
            Code[0x59, 4] = "E\x00f2";
            Code[90, 4] = "\x00ca";
            Code[0x5b, 4] = "\x00ca\x00ec";
            Code[0x5c, 4] = "\x00ca\x00cc";
            Code[0x5d, 4] = "\x00ca\x00d2";
            Code[0x5e, 4] = "\x00ca\x00de";
            Code[0x5f, 4] = "\x00ca\x00f2";
            Code[0x60, 4] = "I\x00ec";
            Code[0x61, 4] = "I\x00cc";
            Code[0x62, 4] = "I\x00d2";
            Code[0x63, 4] = "I\x00de";
            Code[100, 4] = "I\x00f2";
            Code[0x65, 4] = "O\x00ec";
            Code[0x66, 4] = "O\x00cc";
            Code[0x67, 4] = "O\x00d2";
            Code[0x68, 4] = "O\x00de";
            Code[0x69, 4] = "O\x00f2";
            Code[0x6a, 4] = "\x00d4";
            Code[0x6b, 4] = "\x00d4\x00ec";
            Code[0x6c, 4] = "\x00d4\x00cc";
            Code[0x6d, 4] = "\x00d4\x00d2";
            Code[110, 4] = "\x00d4\x00de";
            Code[0x6f, 4] = "\x00d4\x00f2";
            Code[0x70, 4] = "\x00d5";
            Code[0x71, 4] = "\x00d5\x00ec";
            Code[0x72, 4] = "\x00d5\x00cc";
            Code[0x73, 4] = "\x00d5\x00d2";
            Code[0x74, 4] = "\x00d5\x00de";
            Code[0x75, 4] = "\x00d5\x00f2";
            Code[0x76, 4] = "U\x00ec";
            Code[0x77, 4] = "U\x00cc";
            Code[120, 4] = "U\x00d2";
            Code[0x79, 4] = "U\x00de";
            Code[0x7a, 4] = "U\x00f2";
            Code[0x7b, 4] = "\x00dd";
            Code[0x7c, 4] = "\x00dd\x00ec";
            Code[0x7d, 4] = "\x00dd\x00cc";
            Code[0x7e, 4] = "\x00dd\x00d2";
            Code[0x7f, 4] = "\x00dd\x00de";
            Code[0x80, 4] = "\x00dd\x00f2";
            Code[0x81, 4] = "Y\x00ec";
            Code[130, 4] = "Y\x00cc";
            Code[0x83, 4] = "Y\x00d2";
            Code[0x84, 4] = "Y\x00de";
            Code[0x85, 4] = "Y\x00f2";
            Code[0x86, 4] = "\x00d0";
        }

        private void MapCString()
        {
            Code[1, 6] = "\x00e1";
            Code[2, 6] = "\x00e0";
            Code[3, 6] = "?";
            Code[4, 6] = "\x00e3";
            Code[5, 6] = "?";
            Code[6, 6] = "a";
            Code[7, 6] = "?";
            Code[8, 6] = "?";
            Code[9, 6] = "?";
            Code[10, 6] = "?";
            Code[11, 6] = "?";
            Code[12, 6] = "\x00e2";
            Code[13, 6] = "?";
            Code[14, 6] = "?";
            Code[15, 6] = "?";
            Code[0x10, 6] = "?";
            Code[0x11, 6] = "?";
            Code[0x12, 6] = "\x00e9";
            Code[0x13, 6] = "\x00e8";
            Code[20, 6] = "?";
            Code[0x15, 6] = "?";
            Code[0x16, 6] = "?";
            Code[0x17, 6] = "\x00ea";
            Code[0x18, 6] = "?";
            Code[0x19, 6] = "?";
            Code[0x1a, 6] = "?";
            Code[0x1b, 6] = "?";
            Code[0x1c, 6] = "?";
            Code[0x1d, 6] = "\x00ed";
            Code[30, 6] = "\x00ec";
            Code[0x1f, 6] = "?";
            Code[0x20, 6] = "i";
            Code[0x21, 6] = "?";
            Code[0x22, 6] = "\x00f3";
            Code[0x23, 6] = "\x00f2";
            Code[0x24, 6] = "?";
            Code[0x25, 6] = "\x00f5";
            Code[0x26, 6] = "?";
            Code[0x27, 6] = "\x00f4";
            Code[40, 6] = "?";
            Code[0x29, 6] = "?";
            Code[0x2a, 6] = "?";
            Code[0x2b, 6] = "?";
            Code[0x2c, 6] = "?";
            Code[0x2d, 6] = "o";
            Code[0x2e, 6] = "?";
            Code[0x2f, 6] = "?";
            Code[0x30, 6] = "?";
            Code[0x31, 6] = "?";
            Code[50, 6] = "?";
            Code[0x33, 6] = "\x00fa";
            Code[0x34, 6] = "\x00f9";
            Code[0x35, 6] = "?";
            Code[0x36, 6] = "u";
            Code[0x37, 6] = "?";
            Code[0x38, 6] = "u";
            Code[0x39, 6] = "?";
            Code[0x3a, 6] = "?";
            Code[0x3b, 6] = "?";
            Code[60, 6] = "?";
            Code[0x3d, 6] = "?";
            Code[0x3e, 6] = "\x00fd";
            Code[0x3f, 6] = "?";
            Code[0x40, 6] = "?";
            Code[0x41, 6] = "?";
            Code[0x42, 6] = "?";
            Code[0x43, 6] = "d";
            Code[0x44, 6] = "\x00c1";
            Code[0x45, 6] = "\x00c0";
            Code[70, 6] = "?";
            Code[0x47, 6] = "\x00c3";
            Code[0x48, 6] = "?";
            Code[0x49, 6] = "A";
            Code[0x4a, 6] = "?";
            Code[0x4b, 6] = "?";
            Code[0x4c, 6] = "?";
            Code[0x4d, 6] = "?";
            Code[0x4e, 6] = "?";
            Code[0x4f, 6] = "\x00c2";
            Code[80, 6] = "?";
            Code[0x51, 6] = "?";
            Code[0x52, 6] = "?";
            Code[0x53, 6] = "?";
            Code[0x54, 6] = "?";
            Code[0x55, 6] = "\x00c9";
            Code[0x56, 6] = "\x00c8";
            Code[0x57, 6] = "?";
            Code[0x58, 6] = "?";
            Code[0x59, 6] = "?";
            Code[90, 6] = "\x00ca";
            Code[0x5b, 6] = "?";
            Code[0x5c, 6] = "?";
            Code[0x5d, 6] = "?";
            Code[0x5e, 6] = "?";
            Code[0x5f, 6] = "?";
            Code[0x60, 6] = "\x00cd";
            Code[0x61, 6] = "\x00cc";
            Code[0x62, 6] = "?";
            Code[0x63, 6] = "I";
            Code[100, 6] = "?";
            Code[0x65, 6] = "\x00d3";
            Code[0x66, 6] = "\x00d2";
            Code[0x67, 6] = "?";
            Code[0x68, 6] = "\x00d5";
            Code[0x69, 6] = "?";
            Code[0x6a, 6] = "\x00d4";
            Code[0x6b, 6] = "?";
            Code[0x6c, 6] = "?";
            Code[0x6d, 6] = "?";
            Code[110, 6] = "?";
            Code[0x6f, 6] = "?";
            Code[0x70, 6] = "O";
            Code[0x71, 6] = "?";
            Code[0x72, 6] = "?";
            Code[0x73, 6] = "?";
            Code[0x74, 6] = "?";
            Code[0x75, 6] = "?";
            Code[0x76, 6] = "\x00da";
            Code[0x77, 6] = "\x00d9";
            Code[120, 6] = "?";
            Code[0x79, 6] = "U";
            Code[0x7a, 6] = "?";
            Code[0x7b, 6] = "U";
            Code[0x7c, 6] = "?";
            Code[0x7d, 6] = "?";
            Code[0x7e, 6] = "?";
            Code[0x7f, 6] = "?";
            Code[0x80, 6] = "?";
            Code[0x81, 6] = "\x00dd";
            Code[130, 6] = "?";
            Code[0x83, 6] = "?";
            Code[0x84, 6] = "?";
            Code[0x85, 6] = "?";
            Code[0x86, 6] = "Ð";
        }

        private void MapNCR()
        {
            Code[1, 0] = "&#225;";
            Code[2, 0] = "&#224;";
            Code[3, 0] = "&#7843;";
            Code[4, 0] = "&#227;";
            Code[5, 0] = "&#7841;";
            Code[6, 0] = "&#259;";
            Code[7, 0] = "&#7855;";
            Code[8, 0] = "&#7857;";
            Code[9, 0] = "&#7859;";
            Code[10, 0] = "&#7861;";
            Code[11, 0] = "&#7863;";
            Code[12, 0] = "&#226;";
            Code[13, 0] = "&#7845;";
            Code[14, 0] = "&#7847;";
            Code[15, 0] = "&#7849;";
            Code[0x10, 0] = "&#7851;";
            Code[0x11, 0] = "&#7853;";
            Code[0x12, 0] = "&#233;";
            Code[0x13, 0] = "&#232;";
            Code[20, 0] = "&#7867;";
            Code[0x15, 0] = "&#7869;";
            Code[0x16, 0] = "&#7865;";
            Code[0x17, 0] = "&#234;";
            Code[0x18, 0] = "&#7871;";
            Code[0x19, 0] = "&#7873;";
            Code[0x1a, 0] = "&#7875;";
            Code[0x1b, 0] = "&#7877;";
            Code[0x1c, 0] = "&#7879;";
            Code[0x1d, 0] = "&#237;";
            Code[30, 0] = "&#236;";
            Code[0x1f, 0] = "&#7881;";
            Code[0x20, 0] = "&#297;";
            Code[0x21, 0] = "&#7883;";
            Code[0x22, 0] = "&#243;";
            Code[0x23, 0] = "&#242;";
            Code[0x24, 0] = "&#7887;";
            Code[0x25, 0] = "&#245;";
            Code[0x26, 0] = "&#7885;";
            Code[0x27, 0] = "&#244;";
            Code[40, 0] = "&#7889;";
            Code[0x29, 0] = "&#7891;";
            Code[0x2a, 0] = "&#7893;";
            Code[0x2b, 0] = "&#7895;";
            Code[0x2c, 0] = "&#7897;";
            Code[0x2d, 0] = "&#417;";
            Code[0x2e, 0] = "&#7899;";
            Code[0x2f, 0] = "&#7901;";
            Code[0x30, 0] = "&#7903;";
            Code[0x31, 0] = "&#7905;";
            Code[50, 0] = "&#7907;";
            Code[0x33, 0] = "&#250;";
            Code[0x34, 0] = "&#249;";
            Code[0x35, 0] = "&#7911;";
            Code[0x36, 0] = "&#361;";
            Code[0x37, 0] = "&#7909;";
            Code[0x38, 0] = "&#432;";
            Code[0x39, 0] = "&#7913;";
            Code[0x3a, 0] = "&#7915;";
            Code[0x3b, 0] = "&#7917;";
            Code[60, 0] = "&#7919;";
            Code[0x3d, 0] = "&#7921;";
            Code[0x3e, 0] = "&#253;";
            Code[0x3f, 0] = "&#7923;";
            Code[0x40, 0] = "&#7927;";
            Code[0x41, 0] = "&#7929;";
            Code[0x42, 0] = "&#7925;";
            Code[0x43, 0] = "&#273;";
            Code[0x44, 0] = "&#193;";
            Code[0x45, 0] = "&#192;";
            Code[70, 0] = "&#7842;";
            Code[0x47, 0] = "&#195;";
            Code[0x48, 0] = "&#7840;";
            Code[0x49, 0] = "&#258;";
            Code[0x4a, 0] = "&#7854;";
            Code[0x4b, 0] = "&#7856;";
            Code[0x4c, 0] = "&#7858;";
            Code[0x4d, 0] = "&#7860;";
            Code[0x4e, 0] = "&#7862;";
            Code[0x4f, 0] = "&#194;";
            Code[80, 0] = "&#7844;";
            Code[0x51, 0] = "&#7846;";
            Code[0x52, 0] = "&#7848;";
            Code[0x53, 0] = "&#7850;";
            Code[0x54, 0] = "&#7852;";
            Code[0x55, 0] = "&#201;";
            Code[0x56, 0] = "&#200;";
            Code[0x57, 0] = "&#7866;";
            Code[0x58, 0] = "&#7868;";
            Code[0x59, 0] = "&#7864;";
            Code[90, 0] = "&#202;";
            Code[0x5b, 0] = "&#7870;";
            Code[0x5c, 0] = "&#7872;";
            Code[0x5d, 0] = "&#7874;";
            Code[0x5e, 0] = "&#7876;";
            Code[0x5f, 0] = "&#7878;";
            Code[0x60, 0] = "&#205;";
            Code[0x61, 0] = "&#204;";
            Code[0x62, 0] = "&#7880;";
            Code[0x63, 0] = "&#296;";
            Code[100, 0] = "&#7882;";
            Code[0x65, 0] = "&#211;";
            Code[0x66, 0] = "&#210;";
            Code[0x67, 0] = "&#7886;";
            Code[0x68, 0] = "&#213;";
            Code[0x69, 0] = "&#7884;";
            Code[0x6a, 0] = "&#212;";
            Code[0x6b, 0] = "&#7888;";
            Code[0x6c, 0] = "&#7890;";
            Code[0x6d, 0] = "&#7892;";
            Code[110, 0] = "&#7894;";
            Code[0x6f, 0] = "&#7896;";
            Code[0x70, 0] = "&#416;";
            Code[0x71, 0] = "&#7898;";
            Code[0x72, 0] = "&#7900;";
            Code[0x73, 0] = "&#7902;";
            Code[0x74, 0] = "&#7904;";
            Code[0x75, 0] = "&#7906;";
            Code[0x76, 0] = "&#218;";
            Code[0x77, 0] = "&#217;";
            Code[120, 0] = "&#7910;";
            Code[0x79, 0] = "&#360;";
            Code[0x7a, 0] = "&#7908;";
            Code[0x7b, 0] = "&#431;";
            Code[0x7c, 0] = "&#7912;";
            Code[0x7d, 0] = "&#7914;";
            Code[0x7e, 0] = "&#7916;";
            Code[0x7f, 0] = "&#7918;";
            Code[0x80, 0] = "&#7920;";
            Code[0x81, 0] = "&#221;";
            Code[130, 0] = "&#7922;";
            Code[0x83, 0] = "&#7926;";
            Code[0x84, 0] = "&#7928;";
            Code[0x85, 0] = "&#7924;";
            Code[0x86, 0] = "&#272;";
        }

        private void MapNCRHex()
        {
            Code[1, 6] = "\x00e1";
            Code[2, 6] = "\x00e0";
            Code[3, 6] = "&#x1EA3;";
            Code[4, 6] = "\x00e3";
            Code[5, 6] = "&#x1EA1;";
            Code[6, 6] = "&#x103;";
            Code[7, 6] = "&#x1EAF;";
            Code[8, 6] = "&#x1EB1;";
            Code[9, 6] = "&#x1EB3;";
            Code[10, 6] = "&#x1EB5;";
            Code[11, 6] = "&#x1EB7;";
            Code[12, 6] = "\x00e2";
            Code[13, 6] = "&#x1EA5;";
            Code[14, 6] = "&#x1EA7;";
            Code[15, 6] = "&#x1EA9;";
            Code[0x10, 6] = "&#x1EAB;";
            Code[0x11, 6] = "&#x1EAD;";
            Code[0x12, 6] = "\x00e9";
            Code[0x13, 6] = "\x00e8";
            Code[20, 6] = "&#x1EBB;";
            Code[0x15, 6] = "&#x1EBD;";
            Code[0x16, 6] = "&#x1EB9;";
            Code[0x17, 6] = "\x00ea";
            Code[0x18, 6] = "&#x1EBF;";
            Code[0x19, 6] = "&#x1EC1;";
            Code[0x1a, 6] = "&#x1EC3;";
            Code[0x1b, 6] = "&#x1EC5;";
            Code[0x1c, 6] = "&#x1EC7;";
            Code[0x1d, 6] = "\x00ed";
            Code[30, 6] = "\x00ec";
            Code[0x1f, 6] = "&#x1EC9;";
            Code[0x20, 6] = "&#x129;";
            Code[0x21, 6] = "&#x1ECB;";
            Code[0x22, 6] = "\x00f3";
            Code[0x23, 6] = "\x00f2";
            Code[0x24, 6] = "&#x1ECF;";
            Code[0x25, 6] = "\x00f5";
            Code[0x26, 6] = "&#x1ECD;";
            Code[0x27, 6] = "\x00f4";
            Code[40, 6] = "&#x1ED1;";
            Code[0x29, 6] = "&#x1ED3;";
            Code[0x2a, 6] = "&#x1ED5;";
            Code[0x2b, 6] = "&#x1ED7;";
            Code[0x2c, 6] = "&#x1ED9;";
            Code[0x2d, 6] = "&#x1A1;";
            Code[0x2e, 6] = "&#x1EDB;";
            Code[0x2f, 6] = "&#x1EDD;";
            Code[0x30, 6] = "&#x1EDF;";
            Code[0x31, 6] = "&#x1EE1;";
            Code[50, 6] = "&#x1EE3;";
            Code[0x33, 6] = "\x00fa";
            Code[0x34, 6] = "\x00f9";
            Code[0x35, 6] = "&#x1EE7;";
            Code[0x36, 6] = "&#x169;";
            Code[0x37, 6] = "&#x1EE5;";
            Code[0x38, 6] = "&#x1B0;";
            Code[0x39, 6] = "&#x1EE9;";
            Code[0x3a, 6] = "&#x1EEB;";
            Code[0x3b, 6] = "&#x1EED;";
            Code[60, 6] = "&#x1EEF;";
            Code[0x3d, 6] = "&#x1EF1;";
            Code[0x3e, 6] = "\x00fd";
            Code[0x3f, 6] = "&#x1EF3;";
            Code[0x40, 6] = "&#x1EF7;";
            Code[0x41, 6] = "&#x1EF9;";
            Code[0x42, 6] = "&#x1EF5;";
            Code[0x43, 6] = "&#x111;";
            Code[0x44, 6] = "\x00c1";
            Code[0x45, 6] = "\x00c0";
            Code[70, 6] = "&#x1EA2;";
            Code[0x47, 6] = "\x00c3";
            Code[0x48, 6] = "&#x1EA0;";
            Code[0x49, 6] = "&#x102;";
            Code[0x4a, 6] = "&#x1EAE;";
            Code[0x4b, 6] = "&#x1EB0;";
            Code[0x4c, 6] = "&#x1EB2;";
            Code[0x4d, 6] = "&#x1EB4;";
            Code[0x4e, 6] = "&#x1EB6;";
            Code[0x4f, 6] = "\x00c2";
            Code[80, 6] = "&#x1EA4;";
            Code[0x51, 6] = "&#x1EA6;";
            Code[0x52, 6] = "&#x1EA8;";
            Code[0x53, 6] = "&#x1EAA;";
            Code[0x54, 6] = "&#x1EAC;";
            Code[0x55, 6] = "\x00c9";
            Code[0x56, 6] = "\x00c8";
            Code[0x57, 6] = "&#x1EBA;";
            Code[0x58, 6] = "&#x1EBC;";
            Code[0x59, 6] = "&#x1EB8;";
            Code[90, 6] = "\x00ca";
            Code[0x5b, 6] = "&#x1EBE;";
            Code[0x5c, 6] = "&#x1EC0;";
            Code[0x5d, 6] = "&#x1EC2;";
            Code[0x5e, 6] = "&#x1EC4;";
            Code[0x5f, 6] = "&#x1EC6;";
            Code[0x60, 6] = "\x00cd";
            Code[0x61, 6] = "\x00cc";
            Code[0x62, 6] = "&#x1EC8;";
            Code[0x63, 6] = "&#x128;";
            Code[100, 6] = "&#x1ECA;";
            Code[0x65, 6] = "\x00d3";
            Code[0x66, 6] = "\x00d2";
            Code[0x67, 6] = "&#x1ECE;";
            Code[0x68, 6] = "\x00d5";
            Code[0x69, 6] = "&#x1ECC;";
            Code[0x6a, 6] = "\x00d4";
            Code[0x6b, 6] = "&#x1ED0;";
            Code[0x6c, 6] = "&#x1ED2;";
            Code[0x6d, 6] = "&#x1ED4;";
            Code[110, 6] = "&#x1ED6;";
            Code[0x6f, 6] = "&#x1ED8;";
            Code[0x70, 6] = "&#x1A0;";
            Code[0x71, 6] = "&#x1EDA;";
            Code[0x72, 6] = "&#x1EDC;";
            Code[0x73, 6] = "&#x1EDE;";
            Code[0x74, 6] = "&#x1EE0;";
            Code[0x75, 6] = "&#x1EE2;";
            Code[0x76, 6] = "\x00da";
            Code[0x77, 6] = "\x00d9";
            Code[120, 6] = "&#x1EE6;";
            Code[0x79, 6] = "&#x168;";
            Code[0x7a, 6] = "&#x1EE4;";
            Code[0x7b, 6] = "&#x1AF;";
            Code[0x7c, 6] = "&#x1EE8;";
            Code[0x7d, 6] = "&#x1EEA;";
            Code[0x7e, 6] = "&#x1EEC;";
            Code[0x7f, 6] = "&#x1EEE;";
            Code[0x80, 6] = "&#x1EF0;";
            Code[0x81, 6] = "\x00dd";
            Code[130, 6] = "&#x1EF2;";
            Code[0x83, 6] = "&#x1EF6;";
            Code[0x84, 6] = "&#x1EF8;";
            Code[0x85, 6] = "&#x1EF4;";
            Code[0x86, 6] = "&#x110;";
        }

        private void MapNoSign()
        {
            Code[1, 7] = "a";
            Code[2, 7] = "a";
            Code[3, 7] = "a";
            Code[4, 7] = "a";
            Code[5, 7] = "a";
            Code[6, 7] = "a";
            Code[7, 7] = "a";
            Code[8, 7] = "a";
            Code[9, 7] = "a";
            Code[10, 7] = "a";
            Code[11, 7] = "a";
            Code[12, 7] = "a";
            Code[13, 7] = "a";
            Code[14, 7] = "a";
            Code[15, 7] = "a";
            Code[0x10, 7] = "a";
            Code[0x11, 7] = "a";
            Code[0x12, 7] = "e";
            Code[0x13, 7] = "e";
            Code[20, 7] = "e";
            Code[0x15, 7] = "e";
            Code[0x16, 7] = "e";
            Code[0x17, 7] = "e";
            Code[0x18, 7] = "e";
            Code[0x19, 7] = "e";
            Code[0x1a, 7] = "e";
            Code[0x1b, 7] = "e";
            Code[0x1c, 7] = "e";
            Code[0x1d, 7] = "i";
            Code[30, 7] = "i";
            Code[0x1f, 7] = "i";
            Code[0x20, 7] = "i";
            Code[0x21, 7] = "i";
            Code[0x22, 7] = "o";
            Code[0x23, 7] = "o";
            Code[0x24, 7] = "o";
            Code[0x25, 7] = "o";
            Code[0x26, 7] = "o";
            Code[0x27, 7] = "o";
            Code[40, 7] = "o";
            Code[0x29, 7] = "o";
            Code[0x2a, 7] = "o";
            Code[0x2b, 7] = "o";
            Code[0x2c, 7] = "o";
            Code[0x2d, 7] = "o";
            Code[0x2e, 7] = "o";
            Code[0x2f, 7] = "o";
            Code[0x30, 7] = "o";
            Code[0x31, 7] = "o";
            Code[50, 7] = "o";
            Code[0x33, 7] = "u";
            Code[0x34, 7] = "u";
            Code[0x35, 7] = "u";
            Code[0x36, 7] = "u";
            Code[0x37, 7] = "u";
            Code[0x38, 7] = "u";
            Code[0x39, 7] = "u";
            Code[0x3a, 7] = "u";
            Code[0x3b, 7] = "u";
            Code[60, 7] = "u";
            Code[0x3d, 7] = "u";
            Code[0x3e, 7] = "y";
            Code[0x3f, 7] = "y";
            Code[0x40, 7] = "y";
            Code[0x41, 7] = "y";
            Code[0x42, 7] = "y";
            Code[0x43, 7] = "d";
            Code[0x44, 7] = "A";
            Code[0x45, 7] = "A";
            Code[70, 7] = "A";
            Code[0x47, 7] = "A";
            Code[0x48, 7] = "A";
            Code[0x49, 7] = "A";
            Code[0x4a, 7] = "A";
            Code[0x4b, 7] = "A";
            Code[0x4c, 7] = "A";
            Code[0x4d, 7] = "A";
            Code[0x4e, 7] = "A";
            Code[0x4f, 7] = "A";
            Code[80, 7] = "A";
            Code[0x51, 7] = "A";
            Code[0x52, 7] = "A";
            Code[0x53, 7] = "A";
            Code[0x54, 7] = "A";
            Code[0x55, 7] = "E";
            Code[0x56, 7] = "E";
            Code[0x57, 7] = "E";
            Code[0x58, 7] = "E";
            Code[0x59, 7] = "E";
            Code[90, 7] = "E";
            Code[0x5b, 7] = "E";
            Code[0x5c, 7] = "E";
            Code[0x5d, 7] = "E";
            Code[0x5e, 7] = "E";
            Code[0x5f, 7] = "E";
            Code[0x60, 7] = "I";
            Code[0x61, 7] = "I";
            Code[0x62, 7] = "I";
            Code[0x63, 7] = "I";
            Code[100, 7] = "I";
            Code[0x65, 7] = "O";
            Code[0x66, 7] = "O";
            Code[0x67, 7] = "O";
            Code[0x68, 7] = "O";
            Code[0x69, 7] = "O";
            Code[0x6a, 7] = "O";
            Code[0x6b, 7] = "O";
            Code[0x6c, 7] = "O";
            Code[0x6d, 7] = "O";
            Code[110, 7] = "O";
            Code[0x6f, 7] = "O";
            Code[0x70, 7] = "O";
            Code[0x71, 7] = "O";
            Code[0x72, 7] = "O";
            Code[0x73, 7] = "O";
            Code[0x74, 7] = "O";
            Code[0x75, 7] = "O";
            Code[0x76, 7] = "U";
            Code[0x77, 7] = "U";
            Code[120, 7] = "U";
            Code[0x79, 7] = "U";
            Code[0x7a, 7] = "U";
            Code[0x7b, 7] = "U";
            Code[0x7c, 7] = "U";
            Code[0x7d, 7] = "U";
            Code[0x7e, 7] = "U";
            Code[0x7f, 7] = "U";
            Code[0x80, 7] = "U";
            Code[0x81, 7] = "Y";
            Code[130, 7] = "Y";
            Code[0x83, 7] = "Y";
            Code[0x84, 7] = "Y";
            Code[0x85, 7] = "Y";
            Code[0x86, 7] = "D";
        }

        private void MapTCV()
        {
            Code[1, 2] = "\x00b8";
            Code[2, 2] = "\x00b5";
            Code[3, 2] = "\x00b6";
            Code[4, 2] = "\x00b7";
            Code[5, 2] = "\x00b9";
            Code[6, 2] = "\x00a8";
            Code[7, 2] = "\x00be";
            Code[8, 2] = "\x00bb";
            Code[9, 2] = "\x00bc";
            Code[10, 2] = "\x00bd";
            Code[11, 2] = "\x00c6";
            Code[12, 2] = "\x00a9";
            Code[13, 2] = "\x00ca";
            Code[14, 2] = "\x00c7";
            Code[15, 2] = "\x00c8";
            Code[0x10, 2] = "\x00c9";
            Code[0x11, 2] = "\x00cb";
            Code[0x12, 2] = "\x00d0";
            Code[0x13, 2] = "\x00cc";
            Code[20, 2] = "\x00ce";
            Code[0x15, 2] = "\x00cf";
            Code[0x16, 2] = "\x00d1";
            Code[0x17, 2] = "\x00aa";
            Code[0x18, 2] = "\x00d5";
            Code[0x19, 2] = "\x00d2";
            Code[0x1a, 2] = "\x00d3";
            Code[0x1b, 2] = "\x00d4";
            Code[0x1c, 2] = "\x00d6";
            Code[0x1d, 2] = "\x00dd";
            Code[30, 2] = "\x00d7";
            Code[0x1f, 2] = "\x00d8";
            Code[0x20, 2] = "\x00dc";
            Code[0x21, 2] = "\x00de";
            Code[0x22, 2] = "\x00e3";
            Code[0x23, 2] = "\x00df";
            Code[0x24, 2] = "\x00e1";
            Code[0x25, 2] = "\x00e2";
            Code[0x26, 2] = "\x00e4";
            Code[0x27, 2] = "\x00ab";
            Code[40, 2] = "\x00e8";
            Code[0x29, 2] = "\x00e5";
            Code[0x2a, 2] = "\x00e6";
            Code[0x2b, 2] = "\x00e7";
            Code[0x2c, 2] = "\x00e9";
            Code[0x2d, 2] = "\x00ac";
            Code[0x2e, 2] = "\x00ed";
            Code[0x2f, 2] = "\x00ea";
            Code[0x30, 2] = "\x00eb";
            Code[0x31, 2] = "\x00ec";
            Code[50, 2] = "\x00ee";
            Code[0x33, 2] = "\x00f3";
            Code[0x34, 2] = "\x00ef";
            Code[0x35, 2] = "\x00f1";
            Code[0x36, 2] = "\x00f2";
            Code[0x37, 2] = "\x00f4";
            Code[0x38, 2] = "\x00ad";
            Code[0x39, 2] = "\x00f8";
            Code[0x3a, 2] = "\x00f5";
            Code[0x3b, 2] = "\x00f6";
            Code[60, 2] = "\x00f7";
            Code[0x3d, 2] = "\x00f9";
            Code[0x3e, 2] = "\x00fd";
            Code[0x3f, 2] = "\x00fa";
            Code[0x40, 2] = "\x00fb";
            Code[0x41, 2] = "\x00fc";
            Code[0x42, 2] = "\x00fe";
            Code[0x43, 2] = "\x00ae";
            Code[0x44, 2] = "\x00b8";
            Code[0x45, 2] = "\x00b5";
            Code[70, 2] = "\x00b6";
            Code[0x47, 2] = "\x00b7";
            Code[0x48, 2] = "\x00b9";
            Code[0x49, 2] = "\x00a1";
            Code[0x4a, 2] = "\x00be";
            Code[0x4b, 2] = "\x00bb";
            Code[0x4c, 2] = "\x00bc";
            Code[0x4d, 2] = "\x00bd";
            Code[0x4e, 2] = "\x00c6";
            Code[0x4f, 2] = "\x00a2";
            Code[80, 2] = "\x00ca";
            Code[0x51, 2] = "\x00c7";
            Code[0x52, 2] = "\x00c8";
            Code[0x53, 2] = "\x00c9";
            Code[0x54, 2] = "\x00cb";
            Code[0x55, 2] = "\x00d0";
            Code[0x56, 2] = "\x00cc";
            Code[0x57, 2] = "\x00ce";
            Code[0x58, 2] = "\x00cf";
            Code[0x59, 2] = "\x00d1";
            Code[90, 2] = "\x00a3";
            Code[0x5b, 2] = "\x00d5";
            Code[0x5c, 2] = "\x00d2";
            Code[0x5d, 2] = "\x00d3";
            Code[0x5e, 2] = "\x00d4";
            Code[0x5f, 2] = "\x00d6";
            Code[0x60, 2] = "\x00dd";
            Code[0x61, 2] = "\x00d7";
            Code[0x62, 2] = "\x00d8";
            Code[0x63, 2] = "\x00dc";
            Code[100, 2] = "\x00de";
            Code[0x65, 2] = "\x00e3";
            Code[0x66, 2] = "\x00df";
            Code[0x67, 2] = "\x00e1";
            Code[0x68, 2] = "\x00e2";
            Code[0x69, 2] = "\x00e4";
            Code[0x6a, 2] = "\x00a4";
            Code[0x6b, 2] = "\x00e8";
            Code[0x6c, 2] = "\x00e5";
            Code[0x6d, 2] = "\x00e6";
            Code[110, 2] = "\x00e7";
            Code[0x6f, 2] = "\x00e9";
            Code[0x70, 2] = "\x00a5";
            Code[0x71, 2] = "\x00ed";
            Code[0x72, 2] = "\x00ea";
            Code[0x73, 2] = "\x00eb";
            Code[0x74, 2] = "\x00ec";
            Code[0x75, 2] = "\x00ee";
            Code[0x76, 2] = "\x00f3";
            Code[0x77, 2] = "\x00ef";
            Code[120, 2] = "\x00f1";
            Code[0x79, 2] = "\x00f2";
            Code[0x7a, 2] = "\x00f4";
            Code[0x7b, 2] = "\x00a6";
            Code[0x7c, 2] = "\x00f8";
            Code[0x7d, 2] = "\x00f5";
            Code[0x7e, 2] = "\x00f6";
            Code[0x7f, 2] = "\x00f7";
            Code[0x80, 2] = "\x00f9";
            Code[0x81, 2] = "\x00fd";
            Code[130, 2] = "\x00fa";
            Code[0x83, 2] = "\x00fb";
            Code[0x84, 2] = "\x00fc";
            Code[0x85, 2] = "\x00fe";
            Code[0x86, 2] = "\x00a7";
        }

        private void MapHTML()
        {
            Code[1, 20] = "&aacute;";
            Code[2, 20] = "&agrave;";
            Code[3, 20] = "ả";
            Code[4, 20] = "&atilde;";
            Code[5, 20] = "ạ";
            Code[6, 20] = "ă";
            Code[7, 20] = "ắ";
            Code[8, 20] = "ằ";
            Code[9, 20] = "ẳ";
            Code[10, 20] = "ẵ";
            Code[11, 20] = "ặ";
            Code[12, 20] = "&acirc;";
            Code[13, 20] = "ấ";
            Code[14, 20] = "ẫ";
            Code[15, 20] = "ẩ";
            Code[0x10, 20] = "ầ";
            Code[0x11, 20] = "ậ";
            Code[0x12, 20] = "&eacute;";
            Code[0x13, 20] = "&egrave;";
            Code[20, 20] = "ẻ";
            Code[0x15, 20] = "ẽ";
            Code[0x16, 20] = "ẹ";
            Code[0x17, 20] = "&ecirc;";
            Code[0x18, 20] = "ế";
            Code[0x19, 20] = "ề";
            Code[0x1a, 20] = "ể";
            Code[0x1b, 20] = "ễ";
            Code[0x1c, 20] = "ệ";
            Code[0x1d, 20] = "&iacute;";
            Code[30, 20] = "&igrave;";
            Code[0x1f, 20] = "ỉ";
            Code[0x20, 20] = "ĩ";
            Code[0x21, 20] = "ị";
            Code[0x22, 20] = "&oacute;";
            Code[0x23, 20] = "&ograve;";
            Code[0x24, 20] = "ỏ";
            Code[0x25, 20] = "&otilde;";
            Code[0x26, 20] = "ọ";
            Code[0x27, 20] = "&ocirc;";
            Code[40, 20] = "ố";
            Code[0x29, 20] = "ồ";
            Code[0x2a, 20] = "ổ";
            Code[0x2b, 20] = "ỗ";
            Code[0x2c, 20] = "ộ";
            Code[0x2d, 20] = "ơ";
            Code[0x2e, 20] = "ớ";
            Code[0x2f, 20] = "ờ";
            Code[0x30, 20] = "ở";
            Code[0x31, 20] = "ỡ";
            Code[50, 20] = "ợ";
            Code[0x33, 20] = "&uacute;";
            Code[0x34, 20] = "&ugrave;";
            Code[0x35, 20] = "ủ";
            Code[0x36, 20] = "ũ";
            Code[0x37, 20] = "ụ";
            Code[0x38, 20] = "ư";
            Code[0x39, 20] = "ứ";
            Code[0x3a, 20] = "ừ";
            Code[0x3b, 20] = "ử";
            Code[60, 20] = "ữ";
            Code[0x3d, 20] = "ự";
            Code[0x3e, 20] = "&yacute;";
            Code[0x3f, 20] = "ỳ";
            Code[0x40, 20] = "ỷ";
            Code[0x41, 20] = "ỹ";
            Code[0x42, 20] = "ị";
            Code[0x43, 20] = "đ";
            Code[0x44, 20] = "&aacute;";
            Code[0x45, 20] = "&agrave;";
            Code[70, 20] = "ả";
            Code[0x47, 20] = "&atilde;";
            Code[0x48, 20] = "ạ";
            Code[0x49, 20] = "Ă";
            Code[0x4a, 20] = "ắ";
            Code[0x4b, 20] = "ằ";
            Code[0x4c, 20] = "ẳ";
            Code[0x4d, 20] = "ẵ";
            Code[0x4e, 20] = "ặ";
            Code[0x4f, 20] = "&Acirc;";
            Code[80, 20] = "ấ";
            Code[0x51, 20] = "ầ";
            Code[0x52, 20] = "ẩ";
            Code[0x53, 20] = "ẫ";
            Code[0x54, 20] = "ợ";
            Code[0x55, 20] = "&eacute;";
            Code[0x56, 20] = "&egrave;";
            Code[0x57, 20] = "ẻ";
            Code[0x58, 20] = "ẽ";
            Code[0x59, 20] = "ẹ";
            Code[90, 20] = "&Ecirc;";
            Code[0x5b, 20] = "ế";
            Code[0x5c, 20] = "ề";
            Code[0x5d, 20] = "ể";
            Code[0x5e, 20] = "ễ";
            Code[0x5f, 20] = "ệ";
            Code[0x60, 20] = "&iacute;";
            Code[0x61, 20] = "&igrave;";
            Code[0x62, 20] = "ỉ";
            Code[0x63, 20] = "ĩ";
            Code[100, 20] = "ị";
            Code[0x65, 20] = "&oacute;";
            Code[0x66, 20] = "&ograve;";
            Code[0x67, 20] = "ỏ";
            Code[0x68, 20] = "&otilde;";
            Code[0x69, 20] = "ọ";
            Code[0x6a, 20] = "&Ocirc;";
            Code[0x6b, 20] = "ố";
            Code[0x6c, 20] = "ồ";
            Code[0x6d, 20] = "ổ";
            Code[110, 20] = "ỗ";
            Code[0x6f, 20] = "ộ";
            Code[0x70, 20] = "Ơ";
            Code[0x71, 20] = "ớ";
            Code[0x72, 20] = "ờ";
            Code[0x73, 20] = "ở";
            Code[0x74, 20] = "ỡ";
            Code[0x75, 20] = "ợ";
            Code[0x76, 20] = "&uacute;";
            Code[0x77, 20] = "&ugrave;";
            Code[120, 20] = "ủ";
            Code[0x79, 20] = "ũ";
            Code[0x7a, 20] = "ụ";
            Code[0x7b, 20] = "Ư";
            Code[0x7c, 20] = "ứ";
            Code[0x7d, 20] = "ừ";
            Code[0x7e, 20] = "ử";
            Code[0x7f, 20] = "ữ";
            Code[0x80, 20] = "ự";
            Code[0x81, 20] = "&yacute;";
            Code[130, 20] = "ỳ";
            Code[0x83, 20] = "ỷ";
            Code[0x84, 20] = "ỹ";
            Code[0x85, 20] = "ỵ";
            Code[0x86, 20] = "Đ";
        }

        private void MapUnicode()
        {
            Code[1, 6] = "á";
            Code[2, 6] = "à";
            Code[3, 6] = "ả";
            Code[4, 6] = "ã";
            Code[5, 6] = "ạ";
            Code[6, 6] = "ă";
            Code[7, 6] = "ắ";
            Code[8, 6] = "ằ";
            Code[9, 6] = "ẳ";
            Code[10, 6] = "ẵ";
            Code[11, 6] = "ặ";
            Code[12, 6] = "â";
            Code[13, 6] = "ấ";
            Code[14, 6] = "ẫ";
            Code[15, 6] = "ẩ";
            Code[0x10, 6] = "ầ";
            Code[0x11, 6] = "ậ";
            Code[0x12, 6] = "é";
            Code[0x13, 6] = "è";
            Code[20, 6] = "ẻ";
            Code[0x15, 6] = "ẽ";
            Code[0x16, 6] = "ẹ";
            Code[0x17, 6] = "ê";
            Code[0x18, 6] = "ế";
            Code[0x19, 6] = "ề";
            Code[0x1a, 6] = "ể";
            Code[0x1b, 6] = "ễ";
            Code[0x1c, 6] = "ệ";
            Code[0x1d, 6] = "í";
            Code[30, 6] = "ì";
            Code[0x1f, 6] = "ỉ";
            Code[0x20, 6] = "ĩ";
            Code[0x21, 6] = "ị";
            Code[0x22, 6] = "ó";
            Code[0x23, 6] = "ò";
            Code[0x24, 6] = "ỏ";
            Code[0x25, 6] = "õ";
            Code[0x26, 6] = "ọ";
            Code[0x27, 6] = "ô";
            Code[40, 6] = "ố";
            Code[0x29, 6] = "ồ";
            Code[0x2a, 6] = "ổ";
            Code[0x2b, 6] = "ỗ";
            Code[0x2c, 6] = "ộ";
            Code[0x2d, 6] = "ơ";
            Code[0x2e, 6] = "ớ";
            Code[0x2f, 6] = "ờ";
            Code[0x30, 6] = "ở";
            Code[0x31, 6] = "ỡ";
            Code[50, 6] = "ợ";
            Code[0x33, 6] = "ú";
            Code[0x34, 6] = "ù";
            Code[0x35, 6] = "ủ";
            Code[0x36, 6] = "ũ";
            Code[0x37, 6] = "ụ";
            Code[0x38, 6] = "ư";
            Code[0x39, 6] = "ứ";
            Code[0x3a, 6] = "ừ";
            Code[0x3b, 6] = "ử";
            Code[60, 6] = "ữ";
            Code[0x3d, 6] = "ự";
            Code[0x3e, 6] = "ý";
            Code[0x3f, 6] = "ỳ";
            Code[0x40, 6] = "ỷ";
            Code[0x41, 6] = "ỹ";
            Code[0x42, 6] = "ị";
            Code[0x43, 6] = "đ";
            Code[0x44, 6] = "á";
            Code[0x45, 6] = "à";
            Code[70, 6] = "ả";
            Code[0x47, 6] = "ã";
            Code[0x48, 6] = "ạ";
            Code[0x49, 6] = "Ă";
            Code[0x4a, 6] = "ắ";
            Code[0x4b, 6] = "ằ";
            Code[0x4c, 6] = "ẳ";
            Code[0x4d, 6] = "ẵ";
            Code[0x4e, 6] = "ặ";
            Code[0x4f, 6] = "Â";
            Code[80, 6] = "ấ";
            Code[0x51, 6] = "ầ";
            Code[0x52, 6] = "ẩ";
            Code[0x53, 6] = "ẫ";
            Code[0x54, 6] = "ợ";
            Code[0x55, 6] = "é";
            Code[0x56, 6] = "è";
            Code[0x57, 6] = "ẻ";
            Code[0x58, 6] = "ẽ";
            Code[0x59, 6] = "ẹ";
            Code[90, 6] = "Ê";
            Code[0x5b, 6] = "ế";
            Code[0x5c, 6] = "ề";
            Code[0x5d, 6] = "ể";
            Code[0x5e, 6] = "ễ";
            Code[0x5f, 6] = "ệ";
            Code[0x60, 6] = "í";
            Code[0x61, 6] = "ì";
            Code[0x62, 6] = "ỉ";
            Code[0x63, 6] = "ĩ";
            Code[100, 6] = "ị";
            Code[0x65, 6] = "ó";
            Code[0x66, 6] = "ò";
            Code[0x67, 6] = "ỏ";
            Code[0x68, 6] = "õ";
            Code[0x69, 6] = "ọ";
            Code[0x6a, 6] = "Ô";
            Code[0x6b, 6] = "ố";
            Code[0x6c, 6] = "ồ";
            Code[0x6d, 6] = "ổ";
            Code[110, 6] = "ỗ";
            Code[0x6f, 6] = "ộ";
            Code[0x70, 6] = "Ơ";
            Code[0x71, 6] = "ớ";
            Code[0x72, 6] = "ờ";
            Code[0x73, 6] = "ở";
            Code[0x74, 6] = "ỡ";
            Code[0x75, 6] = "ợ";
            Code[0x76, 6] = "ú";
            Code[0x77, 6] = "ù";
            Code[120, 6] = "ủ";
            Code[0x79, 6] = "ũ";
            Code[0x7a, 6] = "ụ";
            Code[0x7b, 6] = "Ư";
            Code[0x7c, 6] = "ứ";
            Code[0x7d, 6] = "ừ";
            Code[0x7e, 6] = "ử";
            Code[0x7f, 6] = "ữ";
            Code[0x80, 6] = "ự";
            Code[0x81, 6] = "ý";
            Code[130, 6] = "ỳ";
            Code[0x83, 6] = "ỷ";
            Code[0x84, 6] = "ỹ";
            Code[0x85, 6] = "ỵ";
            Code[0x86, 6] = "Đ";
        }

        private void MapUTF8()
        {
            Code[1, 1] = "\x00c3\x00a1";
            Code[2, 1] = "\x00c3\x00a0";
            Code[3, 1] = "\x00e1\x00ba\x00a3";
            Code[4, 1] = "\x00c3\x00a3";
            Code[5, 1] = "\x00e1\x00ba\x00a1";
            Code[6, 1] = "\x00c4ƒ";
            Code[7, 1] = "\x00e1\x00ba\x00af";
            Code[8, 1] = "\x00e1\x00ba\x00b1";
            Code[9, 1] = "\x00e1\x00ba\x00b3";
            Code[10, 1] = "\x00e1\x00ba\x00b5";
            Code[11, 1] = "\x00e1\x00ba\x00b7";
            Code[12, 1] = "\x00c3\x00a2";
            Code[13, 1] = "\x00e1\x00ba\x00a5";
            Code[14, 1] = "\x00e1\x00ba\x00a7";
            Code[15, 1] = "\x00e1\x00ba\x00a9";
            Code[0x10, 1] = "\x00e1\x00ba\x00ab";
            Code[0x11, 1] = "\x00e1\x00ba\x00ad";
            Code[0x12, 1] = "\x00c3\x00a9";
            Code[0x13, 1] = "\x00c3\x00a8";
            Code[20, 1] = "\x00e1\x00ba\x00bb";
            Code[0x15, 1] = "\x00e1\x00ba\x00bd";
            Code[0x16, 1] = "\x00e1\x00ba\x00b9";
            Code[0x17, 1] = "\x00c3\x00aa";
            Code[0x18, 1] = "\x00e1\x00ba\x00bf";
            Code[0x19, 1] = "\x00e1\x00bb\x0081";
            Code[0x1a, 1] = "\x00e1\x00bbƒ";
            Code[0x1b, 1] = "\x00e1\x00bb…";
            Code[0x1c, 1] = "\x00e1\x00bb‡";
            Code[0x1d, 1] = "\x00c3\x00ad";
            Code[30, 1] = "\x00c3\x00ac";
            Code[0x1f, 1] = "\x00e1\x00bb‰";
            Code[0x20, 1] = "\x00c4\x00a9";
            Code[0x21, 1] = "\x00e1\x00bb‹";
            Code[0x22, 1] = "\x00c3\x00b3";
            Code[0x23, 1] = "\x00c3\x00b2";
            Code[0x24, 1] = "\x00e1\x00bb\x008f";
            Code[0x25, 1] = "\x00c3\x00b5";
            Code[0x26, 1] = "\x00e1\x00bb\x008d";
            Code[0x27, 1] = "\x00c3\x00b4";
            Code[40, 1] = "\x00e1\x00bb‘";
            Code[0x29, 1] = "\x00e1\x00bb“";
            Code[0x2a, 1] = "\x00e1\x00bb•";
            Code[0x2b, 1] = "\x00e1\x00bb—";
            Code[0x2c, 1] = "\x00e1\x00bb™";
            Code[0x2d, 1] = "\x00c6\x00a1";
            Code[0x2e, 1] = "\x00e1\x00bb›";
            Code[0x2f, 1] = "\x00e1\x00bb\x009d";
            Code[0x30, 1] = "\x00e1\x00bbŸ";
            Code[0x31, 1] = "\x00e1\x00bb\x00a1";
            Code[50, 1] = "\x00e1\x00bb\x00a3";
            Code[0x33, 1] = "\x00c3\x00ba";
            Code[0x34, 1] = "\x00c3\x00b9";
            Code[0x35, 1] = "\x00e1\x00bb\x00a7";
            Code[0x36, 1] = "\x00c5\x00a9";
            Code[0x37, 1] = "\x00e1\x00bb\x00a5";
            Code[0x38, 1] = "\x00c6\x00b0";
            Code[0x39, 1] = "\x00e1\x00bb\x00a9";
            Code[0x3a, 1] = "\x00e1\x00bb\x00ab";
            Code[0x3b, 1] = "\x00e1\x00bb\x00ad";
            Code[60, 1] = "\x00e1\x00bb\x00af";
            Code[0x3d, 1] = "\x00e1\x00bb\x00b1";
            Code[0x3e, 1] = "\x00c3\x00bd";
            Code[0x3f, 1] = "\x00e1\x00bb\x00b3";
            Code[0x40, 1] = "\x009d\x00e1\x00bb\x00b7".Substring(1);
            Code[0x41, 1] = "\x00e1\x00bb\x00b9";
            Code[0x42, 1] = "\x00e1\x00bb\x00b5";
            Code[0x43, 1] = "\x00c4‘";
            Code[0x44, 1] = "\x00c3\x0081";
            Code[0x45, 1] = "\x00c3€";
            Code[70, 1] = "\x00e1\x00ba\x00a2";
            Code[0x47, 1] = "\x00c3ƒ";
            Code[0x48, 1] = "\x00e1\x00ba ";
            Code[0x49, 1] = "\x00c4‚";
            Code[0x4a, 1] = "\x00e1\x00ba\x00ae";
            Code[0x4b, 1] = "\x00e1\x00ba\x00b0";
            Code[0x4c, 1] = "\x00e1\x00ba\x00b2";
            Code[0x4d, 1] = "\x00e1\x00ba\x00b4";
            Code[0x4e, 1] = "\x00e1\x00ba\x00b6";
            Code[0x4f, 1] = "\x00c3‚";
            Code[80, 1] = "\x00e1\x00ba\x00a4";
            Code[0x51, 1] = "\x00e1\x00ba\x00a6";
            Code[0x52, 1] = "\x00e1\x00ba\x00a8";
            Code[0x53, 1] = "\x00e1\x00ba\x00aa";
            Code[0x54, 1] = "\x00e1\x00ba\x00ac";
            Code[0x55, 1] = "\x00c3‰";
            Code[0x56, 1] = "\x00c3ˆ";
            Code[0x57, 1] = "\x00e1\x00ba\x00ba";
            Code[0x58, 1] = "\x00e1\x00ba\x00bc";
            Code[0x59, 1] = "\x00e1\x00ba\x00b8";
            Code[90, 1] = "\x00c3Š";
            Code[0x5b, 1] = "\x00e1\x00ba\x00be";
            Code[0x5c, 1] = "\x00e1\x00bb€";
            Code[0x5d, 1] = "\x00e1\x00bb‚";
            Code[0x5e, 1] = "\x00e1\x00bb„";
            Code[0x5f, 1] = "\x00e1\x00bb†";
            Code[0x60, 1] = "\x00c3\x008d";
            Code[0x61, 1] = "\x00c3Œ";
            Code[0x62, 1] = "\x00e1\x00bbˆ";
            Code[0x63, 1] = "\x00c4\x00a8";
            Code[100, 1] = "\x00e1\x00bbŠ";
            Code[0x65, 1] = "\x00c3“";
            Code[0x66, 1] = "\x00c3’";
            Code[0x67, 1] = "\x00e1\x00bbŽ";
            Code[0x68, 1] = "\x00c3•";
            Code[0x69, 1] = "\x00e1\x00bbŒ";
            Code[0x6a, 1] = "\x00c3”";
            Code[0x6b, 1] = "\x00e1\x00bb\x0090";
            Code[0x6c, 1] = "\x00e1\x00bb’";
            Code[0x6d, 1] = "\x00e1\x00bb”";
            Code[110, 1] = "\x00e1\x00bb–";
            Code[0x6f, 1] = "\x00e1\x00bb˜";
            Code[0x70, 1] = "\x00c6 ";
            Code[0x71, 1] = "\x00e1\x00bbš";
            Code[0x72, 1] = "\x00e1\x00bbœ";
            Code[0x73, 1] = "\x00e1\x00bbž";
            Code[0x74, 1] = "\x00e1\x00bb ";
            Code[0x75, 1] = "\x00e1\x00bb\x00a2";
            Code[0x76, 1] = "\x00c3š";
            Code[0x77, 1] = "\x00c3™";
            Code[120, 1] = "\x00e1\x00bb\x00a6";
            Code[0x79, 1] = "\x00c5\x00a8";
            Code[0x7a, 1] = "\x00e1\x00bb\x00a4";
            Code[0x7b, 1] = "\x00c6\x00af";
            Code[0x7c, 1] = "\x00e1\x00bb\x00a8";
            Code[0x7d, 1] = "\x00e1\x00bb\x00aa";
            Code[0x7e, 1] = "\x00e1\x00bb\x00ac";
            Code[0x7f, 1] = "\x00e1\x00bb\x00ae";
            Code[0x80, 1] = "\x00e1\x00bb\x00b0";
            Code[0x81, 1] = "\x00c3\x009d";
            Code[130, 1] = "\x00e1\x00bb\x00b2";
            Code[0x83, 1] = "\x00e1\x00bb\x00b6";
            Code[0x84, 1] = "\x00e1\x00bb\x00b8";
            Code[0x85, 1] = "\x00e1\x00bb\x00b4";
            Code[0x86, 1] = "\x00c4\x0090";
        }

        private void MapUTH()
        {
            Code[1, 5] = "a´";
            Code[2, 5] = "a`";
            Code[3, 5] = "a?";
            Code[4, 5] = "a~";
            Code[5, 5] = "a?";
            Code[6, 5] = "a";
            Code[7, 5] = "a´";
            Code[8, 5] = "a`";
            Code[9, 5] = "a?";
            Code[10, 5] = "a~";
            Code[11, 5] = "a?";
            Code[12, 5] = "\x00e2";
            Code[13, 5] = "\x00e2´";
            Code[14, 5] = "\x00e2`";
            Code[15, 5] = "\x00e2?";
            Code[0x10, 5] = "\x00e2~";
            Code[0x11, 5] = "\x00e2?";
            Code[0x12, 5] = "e´";
            Code[0x13, 5] = "e`";
            Code[20, 5] = "e?";
            Code[0x15, 5] = "e~";
            Code[0x16, 5] = "e?";
            Code[0x17, 5] = "\x00ea";
            Code[0x18, 5] = "\x00ea´";
            Code[0x19, 5] = "\x00ea`";
            Code[0x1a, 5] = "\x00ea?";
            Code[0x1b, 5] = "\x00ea~";
            Code[0x1c, 5] = "\x00ea?";
            Code[0x1d, 5] = "i´";
            Code[30, 5] = "i`";
            Code[0x1f, 5] = "i?";
            Code[0x20, 5] = "i~";
            Code[0x21, 5] = "i?";
            Code[0x22, 5] = "o´";
            Code[0x23, 5] = "o`";
            Code[0x24, 5] = "o?";
            Code[0x25, 5] = "o~";
            Code[0x26, 5] = "o?";
            Code[0x27, 5] = "\x00f4";
            Code[40, 5] = "\x00f4´";
            Code[0x29, 5] = "\x00f4`";
            Code[0x2a, 5] = "\x00f4?";
            Code[0x2b, 5] = "\x00f4~";
            Code[0x2c, 5] = "\x00f4?";
            Code[0x2d, 5] = "o";
            Code[0x2e, 5] = "o´";
            Code[0x2f, 5] = "o`";
            Code[0x30, 5] = "o?";
            Code[0x31, 5] = "o~";
            Code[50, 5] = "o?";
            Code[0x33, 5] = "u´";
            Code[0x34, 5] = "u`";
            Code[0x35, 5] = "u?";
            Code[0x36, 5] = "u~";
            Code[0x37, 5] = "u?";
            Code[0x38, 5] = "u";
            Code[0x39, 5] = "u´";
            Code[0x3a, 5] = "u`";
            Code[0x3b, 5] = "u?";
            Code[60, 5] = "u~";
            Code[0x3d, 5] = "u?";
            Code[0x3e, 5] = "y´";
            Code[0x3f, 5] = "y`";
            Code[0x40, 5] = "y?";
            Code[0x41, 5] = "y~";
            Code[0x42, 5] = "y?";
            Code[0x43, 5] = "d";
            Code[0x44, 5] = "A´";
            Code[0x45, 5] = "A`";
            Code[70, 5] = "A?";
            Code[0x47, 5] = "A~";
            Code[0x48, 5] = "A?";
            Code[0x49, 5] = "A";
            Code[0x4a, 5] = "A´";
            Code[0x4b, 5] = "A`";
            Code[0x4c, 5] = "A?";
            Code[0x4d, 5] = "A~";
            Code[0x4e, 5] = "A?";
            Code[0x4f, 5] = "\x00c2";
            Code[80, 5] = "\x00c2´";
            Code[0x51, 5] = "\x00c2`";
            Code[0x52, 5] = "\x00c2?";
            Code[0x53, 5] = "\x00c2~";
            Code[0x54, 5] = "\x00c2?";
            Code[0x55, 5] = "E´";
            Code[0x56, 5] = "E`";
            Code[0x57, 5] = "E?";
            Code[0x58, 5] = "E~";
            Code[0x59, 5] = "E?";
            Code[90, 5] = "\x00ca";
            Code[0x5b, 5] = "\x00ca´";
            Code[0x5c, 5] = "\x00ca`";
            Code[0x5d, 5] = "\x00ca?";
            Code[0x5e, 5] = "\x00ca~";
            Code[0x5f, 5] = "\x00ca?";
            Code[0x60, 5] = "I´";
            Code[0x61, 5] = "I`";
            Code[0x62, 5] = "I?";
            Code[0x63, 5] = "I~";
            Code[100, 5] = "I?";
            Code[0x65, 5] = "O´";
            Code[0x66, 5] = "O`";
            Code[0x67, 5] = "O?";
            Code[0x68, 5] = "O~";
            Code[0x69, 5] = "O?";
            Code[0x6a, 5] = "\x00d4";
            Code[0x6b, 5] = "\x00d4´";
            Code[0x6c, 5] = "\x00d4`";
            Code[0x6d, 5] = "\x00d4?";
            Code[110, 5] = "\x00d4~";
            Code[0x6f, 5] = "\x00d4?";
            Code[0x70, 5] = "O";
            Code[0x71, 5] = "O´";
            Code[0x72, 5] = "O`";
            Code[0x73, 5] = "O?";
            Code[0x74, 5] = "O~";
            Code[0x75, 5] = "O?";
            Code[0x76, 5] = "U´";
            Code[0x77, 5] = "U`";
            Code[120, 5] = "U?";
            Code[0x79, 5] = "U~";
            Code[0x7a, 5] = "U?";
            Code[0x7b, 5] = "U";
            Code[0x7c, 5] = "U´";
            Code[0x7d, 5] = "U`";
            Code[0x7e, 5] = "U?";
            Code[0x7f, 5] = "U~";
            Code[0x80, 5] = "U?";
            Code[0x81, 5] = "Y´";
            Code[130, 5] = "Y`";
            Code[0x83, 5] = "Y?";
            Code[0x84, 5] = "Y~";
            Code[0x85, 5] = "Y?";
            Code[0x86, 5] = "Ð";
        }

        private void MapVietwareF()
        {
            Code[1, 6] = "\x00c0";
            Code[2, 6] = "\x00aa";
            Code[3, 6] = "\x00b6";
            Code[4, 6] = "\x00ba";
            Code[5, 6] = "\x00c1";
            Code[6, 6] = "Ÿ";
            Code[7, 6] = "\x00c5";
            Code[8, 6] = "\x00c2";
            Code[9, 6] = "\x00c3";
            Code[10, 6] = "\x00c4";
            Code[11, 6] = "\x00c6";
            Code[12, 6] = "\x00a1";
            Code[13, 6] = "\x00ca";
            Code[14, 6] = "\x00c7";
            Code[15, 6] = "\x00c8";
            Code[0x10, 6] = "\x00c9";
            Code[0x11, 6] = "\x00cb";
            Code[0x12, 6] = "\x00cf";
            Code[0x13, 6] = "\x00cc";
            Code[20, 6] = "\x00cd";
            Code[0x15, 6] = "\x00ce";
            Code[0x16, 6] = "\x00d1";
            Code[0x17, 6] = "\x00a3";
            Code[0x18, 6] = "\x00d5";
            Code[0x19, 6] = "\x00d2";
            Code[0x1a, 6] = "\x00d3";
            Code[0x1b, 6] = "\x00d4";
            Code[0x1c, 6] = "\x00d6";
            Code[0x1d, 6] = "\x00db";
            Code[30, 6] = "\x00d8";
            Code[0x1f, 6] = "\x00d9";
            Code[0x20, 6] = "\x00da";
            Code[0x21, 6] = "\x00dc";
            Code[0x22, 6] = "\x00e2";
            Code[0x23, 6] = "\x00df";
            Code[0x24, 6] = "\x00e0";
            Code[0x25, 6] = "\x00e1";
            Code[0x26, 6] = "\x00e3";
            Code[0x27, 6] = "\x00a4";
            Code[40, 6] = "\x00e7";
            Code[0x29, 6] = "\x00e4";
            Code[0x2a, 6] = "\x00e5";
            Code[0x2b, 6] = "\x00e6";
            Code[0x2c, 6] = "\x00e8";
            Code[0x2d, 6] = "\x00a5";
            Code[0x2e, 6] = "\x00ec";
            Code[0x2f, 6] = "\x00e9";
            Code[0x30, 6] = "\x00ea";
            Code[0x31, 6] = "\x00eb";
            Code[50, 6] = "\x00ed";
            Code[0x33, 6] = "\x00f2";
            Code[0x34, 6] = "\x00ee";
            Code[0x35, 6] = "\x00ef";
            Code[0x36, 6] = "\x00f1";
            Code[0x37, 6] = "\x00f3";
            Code[0x38, 6] = "\x00a7";
            Code[0x39, 6] = "\x00f7";
            Code[0x3a, 6] = "\x00f4";
            Code[0x3b, 6] = "\x00f5";
            Code[60, 6] = "\x00f6";
            Code[0x3d, 6] = "\x00f8";
            Code[0x3e, 6] = "\x00fc";
            Code[0x3f, 6] = "\x00f9";
            Code[0x40, 6] = "\x00fa";
            Code[0x41, 6] = "\x00fb";
            Code[0x42, 6] = "\x00ff";
            Code[0x43, 6] = "\x00a2";
            Code[0x44, 6] = "\x00c0";
            Code[0x45, 6] = "\x00aa";
            Code[70, 6] = "\x00b6";
            Code[0x47, 6] = "\x00ba";
            Code[0x48, 6] = "\x00c1";
            Code[0x49, 6] = "–";
            Code[0x4a, 6] = "\x00c5";
            Code[0x4b, 6] = "\x00c2";
            Code[0x4c, 6] = "\x00c3";
            Code[0x4d, 6] = "\x00c4";
            Code[0x4e, 6] = "\x00c6";
            Code[0x4f, 6] = "—";
            Code[80, 6] = "\x00ca";
            Code[0x51, 6] = "\x00c7";
            Code[0x52, 6] = "\x00c8";
            Code[0x53, 6] = "\x00c9";
            Code[0x54, 6] = "\x00cb";
            Code[0x55, 6] = "\x00cf";
            Code[0x56, 6] = "\x00cc";
            Code[0x57, 6] = "\x00cd";
            Code[0x58, 6] = "\x00ce";
            Code[0x59, 6] = "\x00d1";
            Code[90, 6] = "™";
            Code[0x5b, 6] = "\x00d5";
            Code[0x5c, 6] = "\x00d2";
            Code[0x5d, 6] = "\x00d3";
            Code[0x5e, 6] = "\x00d4";
            Code[0x5f, 6] = "\x00d6";
            Code[0x60, 6] = "\x00db";
            Code[0x61, 6] = "\x00d8";
            Code[0x62, 6] = "\x00d9";
            Code[0x63, 6] = "\x00da";
            Code[100, 6] = "\x00dc";
            Code[0x65, 6] = "\x00e2";
            Code[0x66, 6] = "\x00df";
            Code[0x67, 6] = "\x00e0";
            Code[0x68, 6] = "\x00e1";
            Code[0x69, 6] = "\x00e3";
            Code[0x6a, 6] = "š";
            Code[0x6b, 6] = "\x00e7";
            Code[0x6c, 6] = "\x00e4";
            Code[0x6d, 6] = "\x00e5";
            Code[110, 6] = "\x00e6";
            Code[0x6f, 6] = "\x00e8";
            Code[0x70, 6] = "›";
            Code[0x71, 6] = "\x00ec";
            Code[0x72, 6] = "\x00e9";
            Code[0x73, 6] = "\x00ea";
            Code[0x74, 6] = "\x00eb";
            Code[0x75, 6] = "\x00ed";
            Code[0x76, 6] = "\x00f2";
            Code[0x77, 6] = "\x00ee";
            Code[120, 6] = "\x00ef";
            Code[0x79, 6] = "\x00f1";
            Code[0x7a, 6] = "\x00f3";
            Code[0x7b, 6] = "œ";
            Code[0x7c, 6] = "\x00f7";
            Code[0x7d, 6] = "\x00f4";
            Code[0x7e, 6] = "\x00f5";
            Code[0x7f, 6] = "\x00f6";
            Code[0x80, 6] = "\x00f8";
            Code[0x81, 6] = "\x00fc";
            Code[130, 6] = "\x00f9";
            Code[0x83, 6] = "\x00fa";
            Code[0x84, 6] = "\x00fb";
            Code[0x85, 6] = "\x00ff";
            Code[0x86, 6] = "˜";
        }

        private void MapVietwareX()
        {
            Code[1, 6] = "a\x00ef";
            Code[2, 6] = "a\x00ec";
            Code[3, 6] = "a\x00ed";
            Code[4, 6] = "a\x00ee";
            Code[5, 6] = "a\x00fb";
            Code[6, 6] = "\x00e0";
            Code[7, 6] = "\x00e0\x00f5";
            Code[8, 6] = "\x00e0\x00f2";
            Code[9, 6] = "\x00e0\x00f3";
            Code[10, 6] = "\x00e0\x00f4";
            Code[11, 6] = "\x00e0\x00fb";
            Code[12, 6] = "\x00e1";
            Code[13, 6] = "\x00e1\x00fa";
            Code[14, 6] = "\x00e1\x00f6";
            Code[15, 6] = "\x00e1\x00f8";
            Code[0x10, 6] = "\x00e1\x00f9";
            Code[0x11, 6] = "\x00e1\x00fb";
            Code[0x12, 6] = "e\x00ef";
            Code[0x13, 6] = "e\x00ec";
            Code[20, 6] = "e\x00ed";
            Code[0x15, 6] = "e\x00ee";
            Code[0x16, 6] = "e\x00fb";
            Code[0x17, 6] = "\x00e3";
            Code[0x18, 6] = "\x00e3\x00fa";
            Code[0x19, 6] = "\x00e3\x00f6";
            Code[0x1a, 6] = "\x00e3\x00f8";
            Code[0x1b, 6] = "\x00e3\x00f9";
            Code[0x1c, 6] = "\x00e3\x00fb";
            Code[0x1d, 6] = "\x00ea";
            Code[30, 6] = "\x00e7";
            Code[0x1f, 6] = "\x00e8";
            Code[0x20, 6] = "\x00e9";
            Code[0x21, 6] = "\x00eb";
            Code[0x22, 6] = "o\x00ef";
            Code[0x23, 6] = "o\x00ec";
            Code[0x24, 6] = "o\x00ed";
            Code[0x25, 6] = "o\x00ee";
            Code[0x26, 6] = "o\x00fc";
            Code[0x27, 6] = "\x00e4";
            Code[40, 6] = "\x00e4\x00fa";
            Code[0x29, 6] = "\x00e4\x00f6";
            Code[0x2a, 6] = "\x00e4\x00f8";
            Code[0x2b, 6] = "\x00e4\x00f9";
            Code[0x2c, 6] = "\x00e4\x00fc";
            Code[0x2d, 6] = "\x00e5";
            Code[0x2e, 6] = "\x00e5\x00ef";
            Code[0x2f, 6] = "\x00e5\x00ec";
            Code[0x30, 6] = "\x00e5\x00ed";
            Code[0x31, 6] = "\x00e5\x00ee";
            Code[50, 6] = "\x00e5\x00fc";
            Code[0x33, 6] = "u\x00ef";
            Code[0x34, 6] = "u\x00ec";
            Code[0x35, 6] = "u\x00ed";
            Code[0x36, 6] = "u\x00ee";
            Code[0x37, 6] = "u\x00fb";
            Code[0x38, 6] = "\x00e6";
            Code[0x39, 6] = "\x00e6\x00ef";
            Code[0x3a, 6] = "\x00e6\x00ec";
            Code[0x3b, 6] = "\x00e6\x00ed";
            Code[60, 6] = "\x00e6\x00ee";
            Code[0x3d, 6] = "\x00e6\x00fb";
            Code[0x3e, 6] = "y\x00ef";
            Code[0x3f, 6] = "y\x00ec";
            Code[0x40, 6] = "y\x00ed";
            Code[0x41, 6] = "y\x00ee";
            Code[0x42, 6] = "y\x00f1";
            Code[0x43, 6] = "\x00e2";
            Code[0x44, 6] = "A\x00cf";
            Code[0x45, 6] = "A\x00cc";
            Code[70, 6] = "A\x00cd";
            Code[0x47, 6] = "A\x00ce";
            Code[0x48, 6] = "A\x00db";
            Code[0x49, 6] = "\x00c0";
            Code[0x4a, 6] = "\x00c0\x00d5";
            Code[0x4b, 6] = "\x00c0\x00d2";
            Code[0x4c, 6] = "\x00c0\x00d3";
            Code[0x4d, 6] = "\x00c0\x00d4";
            Code[0x4e, 6] = "\x00c0\x00db";
            Code[0x4f, 6] = "\x00c1";
            Code[80, 6] = "\x00c1\x00da";
            Code[0x51, 6] = "\x00c1\x00d6";
            Code[0x52, 6] = "\x00c1\x00d8";
            Code[0x53, 6] = "\x00c1\x00d9";
            Code[0x54, 6] = "\x00c1\x00db";
            Code[0x55, 6] = "E\x00cf";
            Code[0x56, 6] = "E\x00cc";
            Code[0x57, 6] = "E\x00cd";
            Code[0x58, 6] = "E\x00ce";
            Code[0x59, 6] = "E\x00db";
            Code[90, 6] = "\x00c3";
            Code[0x5b, 6] = "\x00c3\x00da";
            Code[0x5c, 6] = "\x00c3\x00d6";
            Code[0x5d, 6] = "\x00c3\x00d8";
            Code[0x5e, 6] = "\x00c3\x00d9";
            Code[0x5f, 6] = "\x00c3\x00db";
            Code[0x60, 6] = "\x00ca";
            Code[0x61, 6] = "\x00c7";
            Code[0x62, 6] = "\x00c8";
            Code[0x63, 6] = "\x00c9";
            Code[100, 6] = "\x00cb";
            Code[0x65, 6] = "O\x00cf";
            Code[0x66, 6] = "O\x00cc";
            Code[0x67, 6] = "O\x00cd";
            Code[0x68, 6] = "O\x00ce";
            Code[0x69, 6] = "O\x00dc";
            Code[0x6a, 6] = "\x00c4";
            Code[0x6b, 6] = "\x00c4\x00da";
            Code[0x6c, 6] = "\x00c4\x00d6";
            Code[0x6d, 6] = "\x00c4\x00d8";
            Code[110, 6] = "\x00c4\x00d9";
            Code[0x6f, 6] = "\x00c4\x00dc";
            Code[0x70, 6] = "\x00c5";
            Code[0x71, 6] = "\x00c5\x00cf";
            Code[0x72, 6] = "\x00c5\x00cc";
            Code[0x73, 6] = "\x00c5\x00cd";
            Code[0x74, 6] = "\x00c5\x00ce";
            Code[0x75, 6] = "\x00c5\x00dc";
            Code[0x76, 6] = "U\x00cf";
            Code[0x77, 6] = "U\x00cc";
            Code[120, 6] = "U\x00cd";
            Code[0x79, 6] = "U\x00ce";
            Code[0x7a, 6] = "U\x00db";
            Code[0x7b, 6] = "\x00c6";
            Code[0x7c, 6] = "\x00c6\x00cf";
            Code[0x7d, 6] = "\x00c6\x00cc";
            Code[0x7e, 6] = "\x00c6\x00cd";
            Code[0x7f, 6] = "\x00c6\x00ce";
            Code[0x80, 6] = "\x00c6\x00db";
            Code[0x81, 6] = "Y\x00cf";
            Code[130, 6] = "Y\x00cc";
            Code[0x83, 6] = "Y\x00cd";
            Code[0x84, 6] = "Y\x00ce";
            Code[0x85, 6] = "Y\x00d1";
            Code[0x86, 6] = "\x00c2";
        }

        private void MapVIQR()
        {
            Code[1, 8] = "a'";
            Code[2, 8] = "a`";
            Code[3, 8] = "a?";
            Code[4, 8] = "a~";
            Code[5, 8] = "a.";
            Code[6, 8] = "a(";
            Code[7, 8] = "a('";
            Code[8, 8] = "a(`";
            Code[9, 8] = "a(?";
            Code[10, 8] = "a(~";
            Code[11, 8] = "a(.";
            Code[12, 8] = "a^";
            Code[13, 8] = "a^'";
            Code[14, 8] = "a^`";
            Code[15, 8] = "a^?";
            Code[0x10, 8] = "a^~";
            Code[0x11, 8] = "a^.";
            Code[0x12, 8] = "e'";
            Code[0x13, 8] = "e`";
            Code[20, 8] = "e?";
            Code[0x15, 8] = "e~";
            Code[0x16, 8] = "e.";
            Code[0x17, 8] = "e^";
            Code[0x18, 8] = "e^'";
            Code[0x19, 8] = "e^`";
            Code[0x1a, 8] = "e^?";
            Code[0x1b, 8] = "e^~";
            Code[0x1c, 8] = "e^.";
            Code[0x1d, 8] = "i'";
            Code[30, 8] = "i`";
            Code[0x1f, 8] = "i?";
            Code[0x20, 8] = "i~";
            Code[0x21, 8] = "i.";
            Code[0x22, 8] = "o'";
            Code[0x23, 8] = "o`";
            Code[0x24, 8] = "o?";
            Code[0x25, 8] = "o~";
            Code[0x26, 8] = "o.";
            Code[0x27, 8] = "o^";
            Code[40, 8] = "o^'";
            Code[0x29, 8] = "o^`";
            Code[0x2a, 8] = "o^?";
            Code[0x2b, 8] = "o^~";
            Code[0x2c, 8] = "o^.";
            Code[0x2d, 8] = "o+";
            Code[0x2e, 8] = "o+'";
            Code[0x2f, 8] = "o+`";
            Code[0x30, 8] = "o+?";
            Code[0x31, 8] = "o+~";
            Code[50, 8] = "o+.";
            Code[0x33, 8] = "u'";
            Code[0x34, 8] = "u`";
            Code[0x35, 8] = "u?";
            Code[0x36, 8] = "u~";
            Code[0x37, 8] = "u.";
            Code[0x38, 8] = "u+";
            Code[0x39, 8] = "u+'";
            Code[0x3a, 8] = "u+`";
            Code[0x3b, 8] = "u+?";
            Code[60, 8] = "u+~";
            Code[0x3d, 8] = "u+.";
            Code[0x3e, 8] = "y'";
            Code[0x3f, 8] = "y`";
            Code[0x40, 8] = "y?";
            Code[0x41, 8] = "y~";
            Code[0x42, 8] = "y.";
            Code[0x43, 8] = "dd";
            Code[0x44, 8] = "A'";
            Code[0x45, 8] = "A`";
            Code[70, 8] = "A?";
            Code[0x47, 8] = "A~";
            Code[0x48, 8] = "A.";
            Code[0x49, 8] = "A(";
            Code[0x4a, 8] = "A('";
            Code[0x4b, 8] = "A(`";
            Code[0x4c, 8] = "A(?";
            Code[0x4d, 8] = "A(~";
            Code[0x4e, 8] = "A(.";
            Code[0x4f, 8] = "A^";
            Code[80, 8] = "A^'";
            Code[0x51, 8] = "A^`";
            Code[0x52, 8] = "A^?";
            Code[0x53, 8] = "A^~";
            Code[0x54, 8] = "A^.";
            Code[0x55, 8] = "E'";
            Code[0x56, 8] = "E`";
            Code[0x57, 8] = "E?";
            Code[0x58, 8] = "E~";
            Code[0x59, 8] = "E.";
            Code[90, 8] = "E^";
            Code[0x5b, 8] = "E^'";
            Code[0x5c, 8] = "E^`";
            Code[0x5d, 8] = "E^?";
            Code[0x5e, 8] = "E^~";
            Code[0x5f, 8] = "E^.";
            Code[0x60, 8] = "I'";
            Code[0x61, 8] = "I`";
            Code[0x62, 8] = "I?";
            Code[0x63, 8] = "I~";
            Code[100, 8] = "I.";
            Code[0x65, 8] = "O'";
            Code[0x66, 8] = "O`";
            Code[0x67, 8] = "O?";
            Code[0x68, 8] = "O~";
            Code[0x69, 8] = "O.";
            Code[0x6a, 8] = "O^";
            Code[0x6b, 8] = "O^'";
            Code[0x6c, 8] = "O^`";
            Code[0x6d, 8] = "O^?";
            Code[110, 8] = "O^~";
            Code[0x6f, 8] = "O^.";
            Code[0x70, 8] = "O+";
            Code[0x71, 8] = "O+'";
            Code[0x72, 8] = "O+`";
            Code[0x73, 8] = "O+?";
            Code[0x74, 8] = "O+~";
            Code[0x75, 8] = "O+.";
            Code[0x76, 8] = "U'";
            Code[0x77, 8] = "U`";
            Code[120, 8] = "U?";
            Code[0x79, 8] = "U~";
            Code[0x7a, 8] = "U.";
            Code[0x7b, 8] = "U+";
            Code[0x7c, 8] = "U+'";
            Code[0x7d, 8] = "U+`";
            Code[0x7e, 8] = "U+?";
            Code[0x7f, 8] = "U+~";
            Code[0x80, 8] = "U+.";
            Code[0x81, 8] = "Y'";
            Code[130, 8] = "Y`";
            Code[0x83, 8] = "Y?";
            Code[0x84, 8] = "Y~";
            Code[0x85, 8] = "Y.";
            Code[0x86, 8] = "DD";
        }

        private void MapVISCII()
        {
            Code[1, 6] = "\x00e1";
            Code[2, 6] = "\x00e0";
            Code[3, 6] = "\x00e4";
            Code[4, 6] = "\x00e3";
            Code[5, 6] = "\x00d5";
            Code[6, 6] = "\x00e5";
            Code[7, 6] = "\x00a1";
            Code[8, 6] = "\x00a2";
            Code[9, 6] = "\x00c6";
            Code[10, 6] = "\x00c7";
            Code[11, 6] = "\x00a3";
            Code[12, 6] = "\x00e2";
            Code[13, 6] = "\x00a4";
            Code[14, 6] = "\x00a5";
            Code[15, 6] = "\x00a6";
            Code[0x10, 6] = "\x00e7";
            Code[0x11, 6] = "\x00a7";
            Code[0x12, 6] = "\x00e9";
            Code[0x13, 6] = "\x00e8";
            Code[20, 6] = "\x00eb";
            Code[0x15, 6] = "\x00a8";
            Code[0x16, 6] = "\x00a9";
            Code[0x17, 6] = "\x00ea";
            Code[0x18, 6] = "\x00aa";
            Code[0x19, 6] = "\x00ab";
            Code[0x1a, 6] = "\x00ac";
            Code[0x1b, 6] = "\x00ad";
            Code[0x1c, 6] = "\x00ae";
            Code[0x1d, 6] = "\x00ed";
            Code[30, 6] = "\x00ec";
            Code[0x1f, 6] = "\x00ef";
            Code[0x20, 6] = "\x00ee";
            Code[0x21, 6] = "\x00b8";
            Code[0x22, 6] = "\x00f3";
            Code[0x23, 6] = "\x00f2";
            Code[0x24, 6] = "\x00f6";
            Code[0x25, 6] = "\x00f5";
            Code[0x26, 6] = "\x00f7";
            Code[0x27, 6] = "\x00f4";
            Code[40, 6] = "\x00af";
            Code[0x29, 6] = "\x00b0";
            Code[0x2a, 6] = "\x00b1";
            Code[0x2b, 6] = "\x00b2";
            Code[0x2c, 6] = "\x00b5";
            Code[0x2d, 6] = "\x00bd";
            Code[0x2e, 6] = "\x00be";
            Code[0x2f, 6] = "\x00b6";
            Code[0x30, 6] = "\x00b7";
            Code[0x31, 6] = "\x00de";
            Code[50, 6] = "\x00fe";
            Code[0x33, 6] = "\x00fa";
            Code[0x34, 6] = "\x00f9";
            Code[0x35, 6] = "\x00fc";
            Code[0x36, 6] = "\x00fb";
            Code[0x37, 6] = "\x00f8";
            Code[0x38, 6] = "\x00df";
            Code[0x39, 6] = "\x00d1";
            Code[0x3a, 6] = "\x00d7";
            Code[0x3b, 6] = "\x00d8";
            Code[60, 6] = "\x00e6";
            Code[0x3d, 6] = "\x00f1";
            Code[0x3e, 6] = "\x00fd";
            Code[0x3f, 6] = "\x00cf";
            Code[0x40, 6] = "\x00d6";
            Code[0x41, 6] = "\x00db";
            Code[0x42, 6] = "\x00dc";
            Code[0x43, 6] = "\x00f0";
            Code[0x44, 6] = "\x00c1";
            Code[0x45, 6] = "\x00c0";
            Code[70, 6] = "\x00c4";
            Code[0x47, 6] = "\x00c3";
            Code[0x48, 6] = "€";
            Code[0x49, 6] = "\x00c5";
            Code[0x4a, 6] = "\x0081";
            Code[0x4b, 6] = "‚";
            Code[0x4c, 6] = "\x00c6";
            Code[0x4d, 6] = "\x00c7";
            Code[0x4e, 6] = "ƒ";
            Code[0x4f, 6] = "\x00c2";
            Code[80, 6] = "„";
            Code[0x51, 6] = "…";
            Code[0x52, 6] = "†";
            Code[0x53, 6] = "\x00e7";
            Code[0x54, 6] = "‡";
            Code[0x55, 6] = "\x00c9";
            Code[0x56, 6] = "\x00c8";
            Code[0x57, 6] = "\x00cb";
            Code[0x58, 6] = "ˆ";
            Code[0x59, 6] = "‰";
            Code[90, 6] = "\x00ca";
            Code[0x5b, 6] = "Š";
            Code[0x5c, 6] = "‹";
            Code[0x5d, 6] = "Œ";
            Code[0x5e, 6] = "\x008d";
            Code[0x5f, 6] = "Ž";
            Code[0x60, 6] = "\x00cd";
            Code[0x61, 6] = "\x00cc";
            Code[0x62, 6] = "›";
            Code[0x63, 6] = "\x00ce";
            Code[100, 6] = "˜";
            Code[0x65, 6] = "\x00d3";
            Code[0x66, 6] = "\x00d2";
            Code[0x67, 6] = "™";
            Code[0x68, 6] = "\x00f5";
            Code[0x69, 6] = "š";
            Code[0x6a, 6] = "\x00d4";
            Code[0x6b, 6] = "\x008f";
            Code[0x6c, 6] = "\x0090";
            Code[0x6d, 6] = "‘";
            Code[110, 6] = "’";
            Code[0x6f, 6] = "“";
            Code[0x70, 6] = "\x00b4";
            Code[0x71, 6] = "•";
            Code[0x72, 6] = "–";
            Code[0x73, 6] = "—";
            Code[0x74, 6] = "\x00b3";
            Code[0x75, 6] = "”";
            Code[0x76, 6] = "\x00da";
            Code[0x77, 6] = "\x00d9";
            Code[120, 6] = "œ";
            Code[0x79, 6] = "\x009d";
            Code[0x7a, 6] = "ž";
            Code[0x7b, 6] = "\x00bf";
            Code[0x7c, 6] = "\x00ba";
            Code[0x7d, 6] = "\x00bb";
            Code[0x7e, 6] = "\x00bc";
            Code[0x7f, 6] = "\x00ff";
            Code[0x80, 6] = "\x00b9";
            Code[0x81, 6] = "\x00dd";
            Code[130, 6] = "Ÿ";
            Code[0x83, 6] = "\x00d6";
            Code[0x84, 6] = "\x00db";
            Code[0x85, 6] = "\x00dc";
            Code[0x86, 6] = "\x00d0";
        }

        private void MapVNI()
        {
            Code[1, 3] = "a\x00f9";
            Code[2, 3] = "a\x00f8";
            Code[3, 3] = "a\x00fb";
            Code[4, 3] = "a\x00f5";
            Code[5, 3] = "a\x00ef";
            Code[6, 3] = "a\x00ea";
            Code[7, 3] = "a\x00e9";
            Code[8, 3] = "a\x00e8";
            Code[9, 3] = "a\x00fa";
            Code[10, 3] = "a\x00fc";
            Code[11, 3] = "a\x00eb";
            Code[12, 3] = "a\x00e2";
            Code[13, 3] = "a\x00e1";
            Code[14, 3] = "a\x00e0";
            Code[15, 3] = "a\x00e5";
            Code[0x10, 3] = "a\x00e3";
            Code[0x11, 3] = "a\x00e4";
            Code[0x12, 3] = "e\x00f9";
            Code[0x13, 3] = "e\x00f8";
            Code[20, 3] = "e\x00fb";
            Code[0x15, 3] = "e\x00f5";
            Code[0x16, 3] = "e\x00ef";
            Code[0x17, 3] = "e\x00e2";
            Code[0x18, 3] = "e\x00e1";
            Code[0x19, 3] = "e\x00e0";
            Code[0x1a, 3] = "e\x00e5";
            Code[0x1b, 3] = "e\x00e3";
            Code[0x1c, 3] = "e\x00e4";
            Code[0x1d, 3] = "\x00ed";
            Code[30, 3] = "\x00ec";
            Code[0x1f, 3] = "\x00e6";
            Code[0x20, 3] = "\x00f3";
            Code[0x21, 3] = "\x00f2";
            Code[0x22, 3] = "o\x00f9";
            Code[0x23, 3] = "o\x00f8";
            Code[0x24, 3] = "o\x00fb";
            Code[0x25, 3] = "o\x00f5";
            Code[0x26, 3] = "o\x00ef";
            Code[0x27, 3] = "o\x00e2";
            Code[40, 3] = "o\x00e1";
            Code[0x29, 3] = "o\x00e0";
            Code[0x2a, 3] = "o\x00e5";
            Code[0x2b, 3] = "o\x00e3";
            Code[0x2c, 3] = "o\x00e4";
            Code[0x2d, 3] = "\x00f4";
            Code[0x2e, 3] = "\x00f4\x00f9";
            Code[0x2f, 3] = "\x00f4\x00f8";
            Code[0x30, 3] = "\x00f4\x00fb";
            Code[0x31, 3] = "\x00f4\x00f5";
            Code[50, 3] = "\x00f4\x00ef";
            Code[0x33, 3] = "u\x00f9";
            Code[0x34, 3] = "u\x00f8";
            Code[0x35, 3] = "u\x00fb";
            Code[0x36, 3] = "u\x00f5";
            Code[0x37, 3] = "u\x00ef";
            Code[0x38, 3] = "\x00f6";
            Code[0x39, 3] = "\x00f6\x00f9";
            Code[0x3a, 3] = "\x00f6\x00f8";
            Code[0x3b, 3] = "\x00f6\x00fb";
            Code[60, 3] = "\x00f6\x00f5";
            Code[0x3d, 3] = "\x00f6\x00ef";
            Code[0x3e, 3] = "y\x00f9";
            Code[0x3f, 3] = "y\x00f8";
            Code[0x40, 3] = "y\x00fb";
            Code[0x41, 3] = "y\x00f5";
            Code[0x42, 3] = "\x00ee";
            Code[0x43, 3] = "\x00f1";
            Code[0x44, 3] = "A\x00d9";
            Code[0x45, 3] = "A\x00d8";
            Code[70, 3] = "A\x00db";
            Code[0x47, 3] = "A\x00d5";
            Code[0x48, 3] = "A\x00cf";
            Code[0x49, 3] = "A\x00ca";
            Code[0x4a, 3] = "A\x00c9";
            Code[0x4b, 3] = "A\x00c8";
            Code[0x4c, 3] = "A\x00da";
            Code[0x4d, 3] = "A\x00dc";
            Code[0x4e, 3] = "A\x00cb";
            Code[0x4f, 3] = "A\x00c2";
            Code[80, 3] = "A\x00c1";
            Code[0x51, 3] = "A\x00c0";
            Code[0x52, 3] = "A\x00c5";
            Code[0x53, 3] = "A\x00c3";
            Code[0x54, 3] = "A\x00c4";
            Code[0x55, 3] = "E\x00d9";
            Code[0x56, 3] = "E\x00d8";
            Code[0x57, 3] = "E\x00db";
            Code[0x58, 3] = "E\x00d5";
            Code[0x59, 3] = "E\x00cf";
            Code[90, 3] = "E\x00c2";
            Code[0x5b, 3] = "E\x00c1";
            Code[0x5c, 3] = "E\x00c0";
            Code[0x5d, 3] = "E\x00c5";
            Code[0x5e, 3] = "E\x00c3";
            Code[0x5f, 3] = "E\x00c4";
            Code[0x60, 3] = "\x00cd";
            Code[0x61, 3] = "\x00cc";
            Code[0x62, 3] = "\x00c6";
            Code[0x63, 3] = "\x00d3";
            Code[100, 3] = "\x00d2";
            Code[0x65, 3] = "O\x00d9";
            Code[0x66, 3] = "O\x00d8";
            Code[0x67, 3] = "O\x00db";
            Code[0x68, 3] = "O\x00d5";
            Code[0x69, 3] = "O\x00cf";
            Code[0x6a, 3] = "O\x00c2";
            Code[0x6b, 3] = "O\x00c1";
            Code[0x6c, 3] = "O\x00c0";
            Code[0x6d, 3] = "O\x00c5";
            Code[110, 3] = "O\x00c3";
            Code[0x6f, 3] = "O\x00c4";
            Code[0x70, 3] = "\x00d4";
            Code[0x71, 3] = "\x00d4\x00d9";
            Code[0x72, 3] = "\x00d4\x00d8";
            Code[0x73, 3] = "\x00d4\x00db";
            Code[0x74, 3] = "\x00d4\x00d5";
            Code[0x75, 3] = "\x00d4\x00cf";
            Code[0x76, 3] = "U\x00d9";
            Code[0x77, 3] = "U\x00d8";
            Code[120, 3] = "U\x00db";
            Code[0x79, 3] = "U\x00d5";
            Code[0x7a, 3] = "U\x00cf";
            Code[0x7b, 3] = "\x00d6";
            Code[0x7c, 3] = "\x00d6\x00d9";
            Code[0x7d, 3] = "\x00d6\x00d8";
            Code[0x7e, 3] = "\x00d6\x00db";
            Code[0x7f, 3] = "\x00d6\x00d5";
            Code[0x80, 3] = "\x00d6\x00cf";
            Code[0x81, 3] = "Y\x00d9";
            Code[130, 3] = "Y\x00d8";
            Code[0x83, 3] = "Y\x00db";
            Code[0x84, 3] = "Y\x00d5";
            Code[0x85, 3] = "\x00ce";
            Code[0x86, 3] = "\x00d1";
        }

        private void MapVPS()
        {
            Code[1, 6] = "\x00e1";
            Code[2, 6] = "\x00e0";
            Code[3, 6] = "\x00e4";
            Code[4, 6] = "\x00e3";
            Code[5, 6] = "\x00e5";
            Code[6, 6] = "\x00e6";
            Code[7, 6] = "\x00a1";
            Code[8, 6] = "\x00a2";
            Code[9, 6] = "\x00a3";
            Code[10, 6] = "\x00a4";
            Code[11, 6] = "\x00a5";
            Code[12, 6] = "\x00e2";
            Code[13, 6] = "\x00c3";
            Code[14, 6] = "\x00c0";
            Code[15, 6] = "\x00c4";
            Code[0x10, 6] = "\x00c5";
            Code[0x11, 6] = "\x00c6";
            Code[0x12, 6] = "\x00e9";
            Code[0x13, 6] = "\x00e8";
            Code[20, 6] = "\x00c8";
            Code[0x15, 6] = "\x00eb";
            Code[0x16, 6] = "\x00cb";
            Code[0x17, 6] = "\x00ea";
            Code[0x18, 6] = "‰";
            Code[0x19, 6] = "Š";
            Code[0x1a, 6] = "‹";
            Code[0x1b, 6] = "\x00cd";
            Code[0x1c, 6] = "Œ";
            Code[0x1d, 6] = "\x00ed";
            Code[30, 6] = "\x00ec";
            Code[0x1f, 6] = "\x00cc";
            Code[0x20, 6] = "\x00ef";
            Code[0x21, 6] = "\x00ce";
            Code[0x22, 6] = "\x00f3";
            Code[0x23, 6] = "\x00f2";
            Code[0x24, 6] = "\x00d5";
            Code[0x25, 6] = "\x00f5";
            Code[0x26, 6] = "†";
            Code[0x27, 6] = "\x00f4";
            Code[40, 6] = "\x00d3";
            Code[0x29, 6] = "\x00d2";
            Code[0x2a, 6] = "\x00b0";
            Code[0x2b, 6] = "‡";
            Code[0x2c, 6] = "\x00b6";
            Code[0x2d, 6] = "\x00d6";
            Code[0x2e, 6] = "\x00a7";
            Code[0x2f, 6] = "\x00a9";
            Code[0x30, 6] = "\x00aa";
            Code[0x31, 6] = "\x00ab";
            Code[50, 6] = "\x00ae";
            Code[0x33, 6] = "\x00fa";
            Code[0x34, 6] = "\x00f9";
            Code[0x35, 6] = "\x00fb";
            Code[0x36, 6] = "\x00db";
            Code[0x37, 6] = "\x00f8";
            Code[0x38, 6] = "\x00dc";
            Code[0x39, 6] = "\x00d9";
            Code[0x3a, 6] = "\x00d8";
            Code[0x3b, 6] = "\x00ba";
            Code[60, 6] = "\x00bb";
            Code[0x3d, 6] = "\x00bf";
            Code[0x3e, 6] = "š";
            Code[0x3f, 6] = "\x00ff";
            Code[0x40, 6] = "›";
            Code[0x41, 6] = "\x00cf";
            Code[0x42, 6] = "œ";
            Code[0x43, 6] = "\x00c7";
            Code[0x44, 6] = "\x00c1";
            Code[0x45, 6] = "€";
            Code[70, 6] = "\x0081";
            Code[0x47, 6] = "‚";
            Code[0x48, 6] = "\x00e5";
            Code[0x49, 6] = "ˆ";
            Code[0x4a, 6] = "\x008d";
            Code[0x4b, 6] = "Ž";
            Code[0x4c, 6] = "\x008f";
            Code[0x4d, 6] = "\x00f0";
            Code[0x4e, 6] = "\x00a5";
            Code[0x4f, 6] = "\x00c2";
            Code[80, 6] = "ƒ";
            Code[0x51, 6] = "„";
            Code[0x52, 6] = "…";
            Code[0x53, 6] = "\x00c5";
            Code[0x54, 6] = "\x00c6";
            Code[0x55, 6] = "\x00c9";
            Code[0x56, 6] = "\x00d7";
            Code[0x57, 6] = "\x00de";
            Code[0x58, 6] = "\x00fe";
            Code[0x59, 6] = "\x00cb";
            Code[90, 6] = "\x00ca";
            Code[0x5b, 6] = "\x0090";
            Code[0x5c, 6] = "“";
            Code[0x5d, 6] = "”";
            Code[0x5e, 6] = "•";
            Code[0x5f, 6] = "Œ";
            Code[0x60, 6] = "\x00b4";
            Code[0x61, 6] = "\x00b5";
            Code[0x62, 6] = "\x00b7";
            Code[0x63, 6] = "\x00b8";
            Code[100, 6] = "\x00ce";
            Code[0x65, 6] = "\x00b9";
            Code[0x66, 6] = "\x00bc";
            Code[0x67, 6] = "\x00bd";
            Code[0x68, 6] = "\x00be";
            Code[0x69, 6] = "†";
            Code[0x6a, 6] = "\x00d4";
            Code[0x6b, 6] = "–";
            Code[0x6c, 6] = "—";
            Code[0x6d, 6] = "˜";
            Code[110, 6] = "™";
            Code[0x6f, 6] = "\x00b6";
            Code[0x70, 6] = "\x00f7";
            Code[0x71, 6] = "\x009d";
            Code[0x72, 6] = "ž";
            Code[0x73, 6] = "Ÿ";
            Code[0x74, 6] = "\x00a6";
            Code[0x75, 6] = "\x00ae";
            Code[0x76, 6] = "\x00da";
            Code[0x77, 6] = "\x00a8";
            Code[120, 6] = "\x00d1";
            Code[0x79, 6] = "\x00ac";
            Code[0x7a, 6] = "\x00f8";
            Code[0x7b, 6] = "\x00d0";
            Code[0x7c, 6] = "\x00ad";
            Code[0x7d, 6] = "\x00af";
            Code[0x7e, 6] = "\x00b1";
            Code[0x7f, 6] = "\x00bb";
            Code[0x80, 6] = "\x00bf";
            Code[0x81, 6] = "\x00dd";
            Code[130, 6] = "\x00b2";
            Code[0x83, 6] = "\x00fd";
            Code[0x84, 6] = "\x00b3";
            Code[0x85, 6] = "œ";
            Code[0x86, 6] = "\x00f1";
        }

        #endregion

        // Properties
    }

    public enum FontCase
    {
        UpperCase,
        LowerCase,
        Normal
    }

    public enum FontIndex
    {
        iCP1258 = 4,
        iNCR = 0,
        iNOSIGN = 7,
        iNotKnown = -1,
        iTCV = 2,
        iUNI = 6,
        iUTF = 1,
        iUTH = 5,
        iVIQ = 8,
        iVNI = 3,
        iHTML = 20
    }
}