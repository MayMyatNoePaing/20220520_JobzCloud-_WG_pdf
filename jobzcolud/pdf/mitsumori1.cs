using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Drawing;
using GrapeCity.ActiveReports.SectionReportModel;

namespace jobzcolud.pdf
{
    /// <summary>
    /// mitsumori1 の概要の説明です。
    /// </summary>
    public partial class mitsumori1 : GrapeCity.ActiveReports.SectionReport
    {
        private string cHENKOUSYA /*= DEMO20BasicClass.StaticValue.cHENKOUSYA*/;
        public string cMITUMORI { private get; set; }       //見積コード
        public string cMITUMORI_KO { private get; set; }    //見積子番号
        public string sTANTOUSHA { private get; set; }    //担当者名
        public string imagecheck { private get; set; } 
        DataTable dbl_m = new DataTable();   //見積 表数据集
        //MoonLight2.DbLOAD dbl_m_m = new MoonLight2.DbLOAD(); //見積商品明細 表数据集

        DataTable dbl_m_m = new DataTable(); //見積商品明細 
        DataTable dbl_m_ms = new DataTable(); //見積商品明細 詳細サブ

        DataTable dbl_m_s_info = new DataTable(); //書類基本情報 表数据集,
        //private DEMO20BasicClass.Function func = new DEMO20BasicClass.Function();
        private int i = 0;
        public bool fKAKE = true;           //掛け率印刷フラグ
        
        //private const int MAXROWS = 25;     //每页显示的最大行数 
        private const int IMAGEROWS = 9;//画像所占行数
        private const int FIRSTMAXROWS = 28;//見積商品明細数据第一页显示的最大行数[25]
        private const int MAXROWS = 39;     //見積商品明細数据其它页显示的最大行数[36]
        public bool fHYOUSI = false;       //false不打印表紙 True打印表紙
        public bool fNEWIMAGE = false;     //true 最后一页只有图片
        public bool fRYOUHOU = false;// 詳細と一覧両方表示：1，詳細もしくは一覧表示：0
        public int nPAGECOUNT = 0;         //页码
        public int nPAGECOUNT_1 = 0;


        //---------------------------------------------------------------------------------
        double nMITUMORISYOHIZE = 0;    //消费税
        double nKINNGAKUKAZEI = 0;
        public bool fZEINUKIKINNGAKU = false; //税抜金額 
        public bool fZEIFUKUMUKIKINNGAKU = false;
        DataTable dbl_m_j_info = new DataTable();//自社情報マスター表 
        
        private System.Drawing.Font f1 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("10"));
        private System.Drawing.Font f2 = new System.Drawing.Font("HG明朝B", float.Parse("12"));
        private System.Drawing.Font f3 = new System.Drawing.Font("HG明朝B", float.Parse("10"));
        private System.Drawing.Font f4 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("12"));

        private System.Drawing.Font f6 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("9"));
        private System.Drawing.Font f7 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("9.75"));
        private System.Drawing.Font f8 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("10.5"));

        //.......end.........
        //public DEMO20DataClass.r_mitumori_Class rm = new DEMO20DataClass.r_mitumori_Class();
        DataTable dbl_Min = new DataTable();

        public DataTable Syousai_All = new DataTable();//詳細データリストア 
        private DataTable Syousai_Temp1 = new DataTable();//詳細データリストア 
        private DataTable Syousai_Temp2 = new DataTable();//詳細データリストア
        private DataTable Syousai_Temp3 = new DataTable();//詳細データリストア 

        public bool fICHIRAN = false;// 一覧表示：1 
        public bool fSYOUSAI = false;// 詳細表示：1

        public bool fMIDASHI = false;//見出し表示

        public int NEWIMAGE = 0;
        public int fINS = 0;
        private int RowsCount = 0;

        public string HANKO_Check { private get; set; }

        private Boolean header_flag = false;
        //private MoonLight2.DbLOAD dbl_Min = new MoonLight2.DbLOAD();
        //............end..........

        public string frogoimage { private get; set; }
        public string ckyoten { private get; set; }

        public Boolean fkyoten = false;// 拠点マスタで設定してレイアウトを確認するため
        public System.Drawing.Image img { private get; set; }
        public Boolean size_dai { private get; set; }
        public string sTITLE { private get; set; }

        public string tokui_align { private get; set; }
        public string busyo_align { private get; set; }
        public string tantou_align { private get; set; }

        public string sSEIKYU_KEISYO { private get; set; } 
        public string sSEIKYU_YAKUSYOKU { private get; set; }
        public string sTOKUISAKI_TAN { private get; set; }

        public DataTable dt_meisai = new DataTable();
        public mitsumori1()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();

            #region Font
            if (f3.Name != "HG明朝B")
            {
                f3 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("10"));
                //f3 = new System.Drawing.Font("游明朝 Demibold", float.Parse("10"));
            }
            if (f2.Name != "HG明朝B")
            {
                f2 = new System.Drawing.Font("ＭＳ 明朝", float.Parse("12"));
                //f2 = new System.Drawing.Font("游明朝 Demibold", float.Parse("12"));
            }
            #endregion
        }

        private void mitsumori1_ReportStart(object sender, EventArgs e)
        {

        }

        public static int getbyte(string ssyouhin)
        {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            int num = sjisEnc.GetByteCount(ssyouhin);
            return num;
        }

        #region「行数」
        private int LineCount(string str)
        {
            //StringSplitOptions.None including empty row
            //StringSplitOptions.RemoveEmptyEntries not including empty row
            return str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        #endregion

        #region kaigyoucode

        private void kaigyoucode()
        {
            try
            {

                //件名
                Fields["sMITUMORI1"].Value = Fields["sMITUMORI1"].Value.ToString().Trim().Replace("\r\n", "CRLFu000Du000A").Replace("\r", "").Replace("\n", "").Replace("CRLFu000Du000A", "\r\n");

                //得意先
                Fields["sTOKUISAKI"].Value = Fields["sTOKUISAKI"].Value.ToString().Trim().Replace("\r\n", "CRLFu000Du000A").Replace("\r", "").Replace("\n", "").Replace("CRLFu000Du000A", "\r\n");
                //得意先担当者
                Fields["sTOKUISAKI_TAN"].Value = Fields["sTOKUISAKI_TAN"].Value.ToString().Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

                //得意先担当者部門
                Fields["sTOKUISAKI_TANBUMON"].Value = Fields["sTOKUISAKI_TANBUMON"].Value.ToString().Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

            }
            catch { }

        }

        #endregion

        #region  是否有消费税
        private void fZEINUKI(bool fZEIKINNGAKU)
        {
            label8.Visible = fZEIKINNGAKU;
            textBox6.Visible = fZEIKINNGAKU;
            label9.Visible = fZEIKINNGAKU;
            TB_nMITUMORISYOHIZE1.Visible = fZEIKINNGAKU;
            label12.Visible = fZEIKINNGAKU;
            TB_nKINGAKUKAZEI.Visible = fZEIKINNGAKU;
            LB_nKINGAKUKAZEI.Visible = fZEIKINNGAKU;
        }
        #endregion

        private void mitsumori1_ReportEnd(object sender, EventArgs e)
        {
            try
            {
                ;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private void mitsumori1_DataInitialize(object sender, EventArgs e)
        {
            Fields.Add("sCO");
            Fields.Add("sMITUMORI");          //見積欄
            Fields.Add("sNAIYOU");            //内容欄
            Fields.Add("sBIKOU");             //明細備考欄
            Fields.Add("sKEN");               //件名欄
            Fields.Add("sNOUKI");             //納期欄
            Fields.Add("sYUUKOU");            //有効期限欄
            Fields.Add("sSHIHARAI");          //支払条件欄
            Fields.Add("sUKEBASYOU");         //受渡し場所欄
            Fields.Add("sTOKUISAKI");         //得意先名
            Fields.Add("sTOKUISAKI_TAN");     //得意先担当者
            Fields.Add("sMITUMORI1");         //件名
            Fields.Add("dMITUMORINOKI");      //納期
            Fields.Add("sMITUMORIYUKOKIGEN"); //有効期限
            Fields.Add("cSHIHARAI");          //支払条件
            Fields.Add("sUKEWATASIBASYO");    //受渡し場所
            Fields.Add("nRITU");             //掛け率 
            Fields.Add("nMITUMORISYOHIZE");   //消費税
            Fields.Add("nKAZEIKINGAKU"); 
            Fields.Add("sTANTOUSHA");         //担当者
            Fields.Add("nINSATSU_GYO");  
            Fields.Add("sSYOUHIN_R");         //内容.仕様----商品名 
            Fields.Add("nSURYO");             //数量 整数部分
            Fields.Add("nSURYO2");            //数量 小数部分
            Fields.Add("sTANI");              //単位 
            Fields.Add("nTANKA");             //単価
            Fields.Add("nSIKIRITANKA");       //仕切単価 
            Fields.Add("nSIKIRITANKA2");       //仕切単価  
            Fields.Add("nKINGAKU");           //金額           
            Fields.Add("nKINGAKU3");           //金額           
            Fields.Add("nKINGAKU1");          //小計
            Fields.Add("nKINGAKU2");          //お見積合計額
            Fields.Add("nSOUGOUKEI");         //税込金額
            Fields.Add("nMITUMORINEBIKI");         //税込金額
            Fields.Add("nSHIKIRI");             //明細合計
            Fields.Add("nTANKA_G");
            Fields.Add("cYUUBIN");            //郵便番号
            Fields.Add("sJUUSHO1");           //住所１
            Fields.Add("sJUUSHO2");           //住所2
            Fields.Add("sTEL");               //電話番号
            Fields.Add("sFAX");               //ファックス番号
            Fields.Add("sURL");               //ホームページURL
            Fields.Add("sMAIL");              //メールアドレス

            Fields.Add("sTOKUISAKI_TANBUMON");//得意先担当者部門

            Fields.Add("fSAMA");//得意先様、御中フラグ　
            Fields.Add("nKINGAKU_Title_goukei");           //金額  
        }

        #region
        private void Syouhinmei()
        {
            LB_S_sNAIYOU.Alignment = TextAlignment.Left;

            LB_NO.Border.TopStyle = BorderLineStyle.Solid; //行NO
            LB_NO.Border.BottomStyle = BorderLineStyle.Solid;//行NO
            LB_NO.Alignment = TextAlignment.Center;//行NO
            LB_S_sNAIYOU.Border.TopStyle = BorderLineStyle.Solid; //内容.仕様----商品名 
            LB_S_sNAIYOU.Border.BottomStyle = BorderLineStyle.Solid; //内容.仕様----商品名 
            LB_nSURYO.Border.TopStyle = BorderLineStyle.Solid; //数量(整数部分）
            LB_nSURYO.Border.BottomStyle = BorderLineStyle.Solid; //数量(整数部分）
            LB_sTANI.Border.TopStyle = BorderLineStyle.Solid; //単位
            LB_sTANI.Border.BottomStyle = BorderLineStyle.Solid; //単位
            LB_nSIKIRITANKA.Border.TopStyle = BorderLineStyle.Solid;  //単価
            LB_nSIKIRITANKA.Border.BottomStyle = BorderLineStyle.Solid;  //単価
            LB_S_nKINGAKU1.Border.TopStyle = BorderLineStyle.Solid; //金額
            LB_S_nKINGAKU1.Border.BottomStyle = BorderLineStyle.Solid; //金額
            LB_S_sNAIYOU.Border.LeftStyle = BorderLineStyle.Solid;//商品名
            LB_nSURYO.Border.LeftStyle = BorderLineStyle.Solid;
            LB_sTANI.Border.LeftStyle = BorderLineStyle.Solid;
            LB_sTANI.Border.RightStyle = BorderLineStyle.Solid;
            LB_S_nKINGAKU1.Border.LeftStyle = BorderLineStyle.Solid;
            
            if (fSYOUSAI == false && fMIDASHI == false)
            {
                if (fINS == 0)
                {
                    if (dbl_m_m.Rows[i]["sKUBUN"].ToString() == "見")
                    {
                        LB_S_sNAIYOU.Border.LeftStyle = BorderLineStyle.None;
                        LB_nSURYO.Border.LeftStyle = BorderLineStyle.None;
                        LB_sTANI.Border.LeftStyle = BorderLineStyle.None;
                        LB_sTANI.Border.RightStyle = BorderLineStyle.None;
                        LB_S_nKINGAKU1.Border.LeftStyle = BorderLineStyle.None;
                    }
                }
                else
                {
                    if (dbl_Min.Rows[i]["sKUBUN"].ToString() == "見")
                    {
                        LB_S_sNAIYOU.Border.LeftStyle = BorderLineStyle.None;
                        LB_nSURYO.Border.LeftStyle = BorderLineStyle.None;
                        LB_sTANI.Border.LeftStyle = BorderLineStyle.None;
                        LB_sTANI.Border.RightStyle = BorderLineStyle.None;
                        LB_S_nKINGAKU1.Border.LeftStyle = BorderLineStyle.None;
                    }
                }
            }
        }

        private void Syouhinmei_Tsuika(Boolean flag_print, int row)
        {
            ChangeFont(f4);

            //label23.Text = "明細計";
            LB_S_sNAIYOU.Alignment = TextAlignment.Right;
            LB_S_sNAIYOU.Border.LeftStyle = BorderLineStyle.None;
            LB_nSURYO.Border.LeftStyle = BorderLineStyle.None;
            LB_sTANI.Border.LeftStyle = BorderLineStyle.None;
            LB_sTANI.Border.RightStyle = BorderLineStyle.None;
            LB_S_nKINGAKU1.Border.LeftStyle = BorderLineStyle.None;

            if (flag_print == true)//印刷の場合
            {
                if (dbl_m_m.Rows[row]["sSYOUHIN_R"].ToString() == "計")
                {
                    Kei();
                }
                else
                {
                    if (dbl_m_m.Rows[row]["sKUBUN"].ToString() == "計")
                    {
                        SyouKei(false);
                    }
                    else
                    {
                        SyouKei(true);
                    }
                }
            }
            else //プレピューの場合
            {
                if (dbl_Min.Rows[row]["sSYOUHIN_R"].ToString() == "計")
                {
                    Kei();
                }
                else
                {
                    if (dbl_Min.Rows[row]["sKUBUN"].ToString() == "計")
                    {
                        SyouKei(false);
                    }
                    else
                    {
                        SyouKei(true);
                    }
                }

            }
        }
        private void Kei()//計の場合 
        {
            LB_NO.Border.TopStyle = BorderLineStyle.Solid; //行NO
            LB_NO.Alignment = TextAlignment.Center;//行NO
            LB_S_sNAIYOU.Border.LeftStyle = BorderLineStyle.None; //内容.仕様----商品名 
            LB_nSURYO.Border.TopStyle = BorderLineStyle.Solid; //数量(整数部分）
            LB_sTANI.Border.TopStyle = BorderLineStyle.Solid; //単位
            LB_nSIKIRITANKA.Border.TopStyle = BorderLineStyle.Solid;  //単価
            LB_S_nKINGAKU1.Border.TopStyle = BorderLineStyle.Solid; //金額
        }
        private void SyouKei(bool syousai_gyou)//小計の場合 
        {
            if (syousai_gyou == true)
            {
                LB_NO.Border.TopStyle = BorderLineStyle.ThickSolid; //行NO
                LB_S_sNAIYOU.Border.TopStyle = BorderLineStyle.ThickSolid; //内容.仕様----商品名 
                LB_nSURYO.Border.TopStyle = BorderLineStyle.ThickSolid; //数量(整数部分）
                LB_sTANI.Border.TopStyle = BorderLineStyle.ThickSolid; //単位
                LB_nSIKIRITANKA.Border.TopStyle = BorderLineStyle.ThickSolid;  //単価
                LB_S_nKINGAKU1.Border.TopStyle = BorderLineStyle.ThickSolid; //金額
            }
            LB_NO.Border.BottomStyle = BorderLineStyle.ThickSolid;
            LB_NO.Alignment = TextAlignment.Center;//行NO
            LB_S_sNAIYOU.Border.BottomStyle = BorderLineStyle.ThickSolid;
            LB_nSURYO.Border.BottomStyle = BorderLineStyle.ThickSolid;
            LB_sTANI.Border.BottomStyle = BorderLineStyle.ThickSolid;
            LB_nSIKIRITANKA.Border.BottomStyle = BorderLineStyle.ThickSolid;
            LB_S_nKINGAKU1.Border.BottomStyle = BorderLineStyle.ThickSolid;
        }

        private void LastRow()//最後行の設定
        {
            LB_NO.Border.BottomStyle = BorderLineStyle.ThickSolid;           //行NO
            LB_NO.Alignment = TextAlignment.Center;//行NO
            LB_S_sNAIYOU.Border.BottomStyle = BorderLineStyle.ThickSolid;    //内容.仕様----商品名 
            LB_nSURYO.Border.BottomStyle = BorderLineStyle.ThickSolid;       //数量(整数部分）
            LB_nSURYO2.Border.BottomStyle = BorderLineStyle.ThickSolid;      //数量(小数部分
            LB_sTANI.Border.BottomStyle = BorderLineStyle.ThickSolid;        //単位
            LB_nSIKIRITANKA.Border.BottomStyle = BorderLineStyle.ThickSolid;       //単価
            LB_S_nKINGAKU1.Border.BottomStyle = BorderLineStyle.ThickSolid;  //金額
        }
        #endregion

        #region

        int k1 = 1;
        private void pageHeader_BeforePrint(object sender, EventArgs e)
        {
            if (PageNumber < 2)
            {
                label3.Visible = false;
                LB_PAGE_cMITUMORI.Visible = false;
                LB_PAGE_dMITUMORISAKUSEI.Visible = false;

                LB_PAGE1.Visible = false;
            }
            else
            {
                label3.Visible = true;
                LB_PAGE_cMITUMORI.Visible = true;
                //LB_PAGE_dMITUMORISAKUSEI.Visible = true;
                #region
                //if (FRM_RPT_PRINT_CHOICE.fHIDZUKE == true)
                //{
                //    LB_PAGE_dMITUMORISAKUSEI.Visible = true;
                //}
                //else
                //{
                //    LB_PAGE_dMITUMORISAKUSEI.Visible = false;
                //}
                #endregion

                //LB_PAGE_cMITUMORI.Font = f6;
                //LB_PAGE_dMITUMORISAKUSEI.Font = f7;

                //if (header_flag == true && this.PageNumber == nPAGECOUNT_1)
                //{
                //    textBox8.Visible = false;
                //    textBox9.Visible = false;
                //    textBox10.Visible = false;
                //    textBox7.Visible = false;
                //    textBox3.Visible = false;
                //    textBox11.Visible = false;

                //    //this.pageHeader.Height = float.Parse("0");
                //    header_flag = false;

                //    label3.Visible = false;
                //    LB_PAGE_cMITUMORI.Visible = false;
                //    LB_PAGE_dMITUMORISAKUSEI.Visible = false;
                //}
            }

            #region「ページが一つだけの場合」

            if (nPAGECOUNT > 1)
            {
                if (fINS == 0)//印刷
                {
                    //if (FRM_RPT_PRINT_CHOICE.rmt_pcount1 > 0)
                    //{
                    //    LB_PAGE1.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k1) + " / " + nPAGECOUNT.ToString() + ")";
                    //}
                    //else
                    //{
                    //    if (fHYOUSI == true)
                    //    {
                    //        LB_PAGE1.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k1 + 1) + " / " + nPAGECOUNT.ToString() + ")";
                    //    }
                    //    else
                    //    {
                    //        LB_PAGE1.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k1) + " / " + nPAGECOUNT.ToString() + ")";
                    //    }
                    //}
                    k1++;
                }
                else //プレピュー
                {
                    //if (fHYOUSI == true)
                    //{
                    //    LB_PAGE1.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k1 + 1) + " / " + nPAGECOUNT.ToString() + ")";
                    //}
                    //else
                    //{
                    //    LB_PAGE1.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k1) + " / " + nPAGECOUNT.ToString() + ")";
                    //}
                    //k1++;
                }
            }


            #endregion
        }
        int k = 1;
        private void pageFooter_BeforePrint(object sender, EventArgs e)
        {

            if (fINS == 0)//印刷
            {
                //if (FRM_RPT_PRINT_CHOICE.rmt_pcount1 > 0)
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //else
                //{
                //    if (fHYOUSI == true)
                //    {
                //        this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k + 1) + " / " + nPAGECOUNT.ToString() + ")";
                //    }
                //    else
                //    {
                //        this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k) + " / " + nPAGECOUNT.ToString() + ")";
                //    }
                //}
                //k++;
            }
            else //プレピュー
            {
                //if (fHYOUSI == true)
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k + 1) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //else
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //k++;
            }


        }
        #endregion

        private void ChangeFont(System.Drawing.Font fond)
        {
            LB_S_sNAIYOU.Font = fond;
        }

        #region

        private void reportHeader1_BeforePrint(object sender, EventArgs e)
        {
            if (fINS == 0)//印刷
            {
                //if (FRM_RPT_PRINT_CHOICE.rmt_pcount1 > 0)
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //else
                //{
                //    if (fHYOUSI == true)
                //    {
                //        this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k + 1) + " / " + nPAGECOUNT.ToString() + ")";
                //    }
                //    else
                //    {
                //        this.LB_PAGE.Text = "(" + (FRM_RPT_PRINT_CHOICE.rmt_pcount1 + k) + " / " + nPAGECOUNT.ToString() + ")";
                //    }
                //}
                //k++;
            }
            else //プレピュー
            {
                //if (fHYOUSI == true)
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k + 1) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //else
                //{
                //    this.LB_PAGE.Text = "(" + (FRM_SHIJI_VIEW.rmt_pcount + k) + " / " + nPAGECOUNT.ToString() + ")";
                //}
                //k++;

                //if (fkyoten == true)
                //{
                //    this.LB_PAGE.Text = "(1 / 1)";
                //}
            }

        }

        #endregion

        #region show_hankou()
        private void show_hankou()
        {
            DataTable db = new DataTable();
            string sql_new = string.Empty;
            sql_new = "";
            sql_new += "SELECT";
            sql_new += " sTANTOUSHA as sTANTOUSHA";
            sql_new += ",sMAIL AS sMAIL";
            sql_new += ",sIMAGE1 AS sIMAGE1";
            sql_new += " FROM m_j_tantousha";
            sql_new += " WHERE cTANTOUSHA='" + this.cHENKOUSYA + "'";

            //db.Autoitem(sql_new, "m_j_tantousha", DEMO20BasicClass.DBConnector.conn);

            if (HANKO_Check == "欄有り(担当印有り)")
            {
                if (db.Rows.Count > 0)
                {
                    if (db.Rows[0]["sIMAGE1"] != null)
                    {
                        if (!string.IsNullOrEmpty(db.Rows[0]["sIMAGE1"].ToString()))
                        {
                            //P_sIMAGEHankou.Image = Image.FromStream(func.toImage((byte[])db.Rows[0]["sIMAGE1"]));
                            P_sIMAGEHankou.PictureAlignment = PictureAlignment.Center;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
