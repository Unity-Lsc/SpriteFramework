using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Text;

//如果要支持xlsx格式表格，请在本机电脑安装这个
//http://download.microsoft.com/download/7/0/3/703ffbcb-dc0c-4e19-b0da-1463960fdcdb/AccessDatabaseEngine.exe

namespace ExcelTool
{
    class Program
    {
        /// <summary>
        /// 源excel路径
        /// </summary>
        private static string m_SourceExcelPath;

        /// <summary>
        /// 生成的bytes文件路径
        /// </summary>
        private static string m_OutBytesFilePath;

        /// <summary>
        /// 生成的c#脚本路径
        /// </summary>
        private static string m_OutCSharpFilePath;

        /// <summary>
        /// 生成的Lua脚本路径
        /// </summary>
        private static string m_OutLuaFilePath;

        /// <summary>
        /// 生成的服务器端表格文件路径
        /// </summary>
        private static string m_OutBytesFilePath_Server;
        /// <summary>
        /// 生成的服务器端c#脚本路径
        /// </summary>
        private static string m_OutCSharpFilePath_Server;
        /// <summary>
        /// 生成的热更层C#脚本路径
        /// </summary>
        private static string m_OutHotfixFilePath;

        static void Main(string[] args) {
            LoadConfig();
            ReadFiles(m_SourceExcelPath);

            Console.WriteLine("全部生成完毕");
            Console.ReadLine();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void LoadConfig() {
            string configPath = Environment.CurrentDirectory + "/config.txt";
            if (File.Exists(configPath)) {
                string str = "";
                using (FileStream fs = new FileStream(configPath, FileMode.Open)) {
                    using (StreamReader sr = new StreamReader(fs)) {
                        str = sr.ReadToEnd();
                    }
                }

                if (!string.IsNullOrWhiteSpace(str)) {
                    string[] arr = str.Split('\n');
                    m_SourceExcelPath = arr[0].Trim();
                    m_OutBytesFilePath = arr[1].Trim();
                    m_OutCSharpFilePath = arr[2].Trim();
                    if (arr.Length > 3) m_OutLuaFilePath = arr[3].Trim();
                    if (arr.Length > 4) m_OutBytesFilePath_Server = arr[4].Trim();
                    if (arr.Length > 5) m_OutCSharpFilePath_Server = arr[5].Trim();
                    if (arr.Length > 6) m_OutHotfixFilePath = arr[6].Trim();
                }
            }
        }

        /// <summary>
        /// 读取Excel配置文件
        /// </summary>
        /// <param name="path">存放Excel配置文件的文件夹路径</param>
        private static List<string> ReadFiles(string path) {
            string[] arr = Directory.GetFiles(path);

            List<string> lst = new List<string>();

            int len = arr.Length;
            for (int i = 0; i < len; i++) {
                string filePath = arr[i];
                FileInfo file = new FileInfo(filePath);
                //打开Excel的时候,文件夹中会出现~$的临时文件
                if (file.Name.IndexOf("~$") > -1) {
                    continue;
                }
                if (file.Extension.Equals(".xls") || file.Extension.Equals(".xlsx")) {
                    ReadData(file.Extension.Equals(".xls"), file.FullName, file.Name.Substring(0, file.Name.LastIndexOf('.')));
                }
            }

            return lst;
        }

        /// <summary>
        /// 读取每个Excel文件的数据
        /// </summary>
        /// <param name="isXls">是否是.xls格式的文件</param>
        /// <param name="filePath">要读取的Excel文件路径</param>
        /// <param name="fileName">要读取的Excel文件名字(为了给多语言表做单独处理)</param>
        private static void ReadData(bool isXls, string filePath, string fileName) {

            if (string.IsNullOrWhiteSpace(filePath)) return;

            //把表格复制一下
            string newPath = filePath + ".temp";

            File.Copy(filePath, newPath, true);

            string tableName = "Sheet1";
            string strConn;
            if (isXls) {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + newPath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            } else {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =" + newPath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";
            }

            string strExcel = string.Format("select * from [{0}$]", tableName);
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "table1");
            DataTable dt = ds.Tables[0];
            myCommand.Dispose();

            File.Delete(newPath);

            if (fileName.Equals("DTSysLocalization", StringComparison.CurrentCultureIgnoreCase)) {
                //多语言表 单独处理
                CreateLocalization(fileName, dt);
            } else {
                CreateData(fileName, dt);
            }
        }

        #region 普通表的创建

        private static void CreateData(string fileName, DataTable dt) {
            try {
                //数据格式 行数 列数 二维数组每项的值 这里不做判断 都用string存储
                string[,] tableHeadArr = null;
                byte[] buffer = null;

                using (SpriteMemoryStream ms = new SpriteMemoryStream()) {
                    int rows = dt.Rows.Count;
                    for (int i = 0; i < dt.Rows.Count; i++) {
                        //防止空行
                        if (string.IsNullOrWhiteSpace(dt.Rows[i][0].ToString())) {
                            rows = i;
                            break;
                        }
                    }

                    int columns = dt.Columns.Count;
                    for (int i = 0; i < dt.Columns.Count; i++) {
                        //防止空列
                        if (string.IsNullOrWhiteSpace(dt.Rows[0][i].ToString())) {
                            columns = i;
                            break;
                        }
                    }

                    
                    tableHeadArr = new string[columns, 3];

                    ms.WriteInt(rows - 3); //减去表头的三行
                    ms.WriteInt(columns);
                    for (int i = 0; i < rows; i++) {
                        for (int j = 0; j < columns; j++) {
                            if (i < 3) {
                                tableHeadArr[j, i] = dt.Rows[i][j].ToString().Trim();
                            } else {
                                string type = tableHeadArr[j, 1];
                                string value = dt.Rows[i][j].ToString().Trim();

                                //Console.WriteLine("type=" + type + "||" + "value=" + value);

                                switch (type.ToLower()) {
                                    case "int":
                                        ms.WriteInt(string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value));
                                        break;
                                    case "long":
                                        ms.WriteLong(string.IsNullOrWhiteSpace(value) ? 0 : long.Parse(value));
                                        break;
                                    case "short":
                                        ms.WriteShort(string.IsNullOrWhiteSpace(value) ? (short)0 : short.Parse(value));
                                        break;
                                    case "float":
                                        ms.WriteFloat(string.IsNullOrWhiteSpace(value) ? 0 : float.Parse(value));
                                        break;
                                    case "byte":
                                        ms.WriteByte(string.IsNullOrWhiteSpace(value) ? (byte)0 : byte.Parse(value));
                                        break;
                                    case "bool":
                                        ms.WriteBool((string.IsNullOrEmpty(value) || value == "假") ? false : (value == "真" ? true : bool.Parse(value)));
                                        break;
                                    case "double":
                                        ms.WriteDouble(string.IsNullOrWhiteSpace(value) ? 0 : double.Parse(value));
                                        break;
                                    default:
                                        ms.WriteUTF8String(value);
                                        break;
                                }
                            }
                        }
                    }
                    buffer = ms.ToArray();
                }

                CreateEntity(fileName, tableHeadArr, buffer);

            } catch (Exception ex) {
                Console.WriteLine("表格=>" + fileName + " 处理失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建客户端实体
        /// </summary>
        private static void CreateEntity(string fileName, string[,] dataArr, byte[] buffer) {
            if (dataArr == null) return;

            //生成Byte文件
            {
                if (!Directory.Exists(m_OutBytesFilePath)) Directory.CreateDirectory(m_OutBytesFilePath);
                FileStream fs = new FileStream(string.Format("{0}\\{1}", m_OutBytesFilePath, fileName + ".bytes"), FileMode.Create);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                Console.WriteLine("客户端表格=>" + fileName + " 生成bytes文件完毕");
            }

            //生成代码Entity
            StringBuilder sbr = new StringBuilder();
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("\r\n");
            sbr.Append("namespace SpriteFramework\r\n");
            sbr.Append("{\r\n");
            sbr.Append("    /// <summary>\r\n");
            sbr.AppendFormat("      /// {0}实体\r\n", fileName);
            sbr.Append("    /// </summary>\r\n");
            sbr.AppendFormat("    public partial class {0}Entity : DataTableEntityBase\r\n", fileName);
            sbr.Append("    {\r\n");

            for (int i = 0; i < dataArr.GetLength(0); i++) {
                if (i == 0) continue;
                sbr.Append("        /// <summary>\r\n");
                sbr.AppendFormat("        /// {0}\r\n", dataArr[i, 2]);
                sbr.Append("        /// </summary>\r\n");
                sbr.AppendFormat("        public {0} {1};\r\n", dataArr[i, 1], dataArr[i, 0]);
                sbr.Append("\r\n");
            }

            sbr.Append("    }\r\n");
            sbr.Append("}\r\n");


            using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", m_OutCSharpFilePath, fileName), FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.Write(sbr.ToString());
                }
            }

            //生成代码DBModel
            sbr.Clear();
            sbr.Append("\r\n");
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("using System.Collections.Generic;\r\n");
            sbr.Append("using System;\r\n");
            sbr.Append("\r\n");
            sbr.Append("namespace SpriteFramework\r\n");
            sbr.Append("{\r\n");
            sbr.Append("    /// <summary>\r\n");
            sbr.AppendFormat("    /// {0}数据管理\r\n", fileName);
            sbr.Append("    /// </summary>\r\n");
            sbr.AppendFormat("    public partial class {0}DBModel : DataTableDBModelBase<{0}DBModel, {0}Entity>\r\n", fileName);
            sbr.Append("    {\r\n");

            sbr.Append("        /// <summary>\r\n");
            sbr.Append("        /// 文件名称\r\n");
            sbr.Append("        /// </summary>\r\n");
            sbr.AppendFormat("        public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", fileName);
            sbr.Append("\r\n");


            sbr.Append("        /// <summary>\r\n");
            sbr.Append("        /// 加载列表\r\n");
            sbr.Append("        /// </summary>\r\n");
            sbr.Append("        protected override void LoadList(SpriteMemoryStream ms)\r\n");
            sbr.Append("        {\r\n");
            sbr.Append("            int rows = ms.ReadInt();\r\n");
            sbr.Append("            int columns = ms.ReadInt();\r\n");
            sbr.Append("\r\n");
            sbr.Append("            for (int i = 0; i < rows; i++)\r\n");
            sbr.Append("            {\r\n");
            sbr.AppendFormat("                {0}Entity entity = new {0}Entity();\r\n", fileName);

            for (int i = 0; i < dataArr.GetLength(0); i++) {
                if (dataArr[i, 1].Equals("byte", StringComparison.CurrentCultureIgnoreCase)) {
                    sbr.AppendFormat("                entity.{0} = (byte)ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                } else {
                    sbr.AppendFormat("                entity.{0} = ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
            }

            sbr.Append("\r\n");
            sbr.Append("                m_List.Add(entity);\r\n");
            sbr.Append("                m_Dic[entity.Id] = entity;\r\n");
            sbr.Append("            }\r\n");
            sbr.Append("        }\r\n");
            sbr.Append("    }\r\n");

            sbr.Append("}");
            using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", m_OutCSharpFilePath, fileName), FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.Write(sbr.ToString());
                }
            }

            Console.WriteLine("客户端表格=>" + fileName + " 生成实体脚本和实体数据脚本完毕");
        }

        /// <summary>
        /// 将类型名字从小写切换成正常状态
        /// </summary>
        private static string ChangeTypeName(string type) {
            string str = string.Empty;

            switch (type) {
                case "byte": str = "Byte"; break;
                case "int": str = "Int"; break;
                case "short": str = "Short"; break;
                case "long": str = "Long"; break;
                case "float": str = "Float"; break;
                case "string": str = "UTF8String"; break;
                case "bool": str = "Bool"; break;
            }

            return str;
        }

        #endregion 普通表的创建end

        #region 多语言表的创建

        private static void CreateLocalization(string fileName, DataTable dt) {

        }

        #endregion 多语言表的创建end

    }
}
