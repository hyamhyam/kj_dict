// Copyright 2004, 2011 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization ;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.InteropServices; 
using HYAM.Lingua;

namespace HYAM.KJ_dict
{
    // こちらは主にフォームのデザイン
    //  (辞書検索・表示用のロジックはKJ_form.cs側)
    public partial class KJ_form : Form
    {

#if !EDICT
        // サブフォームを１つしか出さないための記憶域
        public  Hangul_Button_Form    hangul_button_instance;
        public  Hiragana_Button_Form  hiragana_button_instance;
        public  Hangul_keyboard_Form  hangul_keyboard_instance;
#endif

        public  Extended_menu_Form    extended_menu_instance;

        public  About_Form            about_menu_instance;

        private static KJ_Message msg;
        
        private Thread translatorThread;
        
        // ハングルキーボード用（入力されたキー文字列）
        public string inputKey = "" ;  
        
        
        // 入力エリア
        public System.Windows.Forms.TextBox inputArea;
         // 韓国IMEに対する動作不良対策のため，
         // 入力エリアをRichTextBoxからTextBoxに変更
         
    //    private System.Windows.Forms.Button exitButton;    // exit
        
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;

        
        // 結果の表示エリア
   //     public RichTextBoxLinks.RichTextBoxEx richTextBox2; (リンク用)
        public System.Windows.Forms.RichTextBox richTextBox2;

        // フォント変更用
        public System.Drawing.Font formFont;
        public System.Drawing.Font formFont_s;
        
        
        // Copy&Paste
        private System.Windows.Forms.ContextMenu cm1;        
        private System.Windows.Forms.MenuItem menuCopy1;     
        private System.Windows.Forms.MenuItem menuPaste1;    
        private System.Windows.Forms.ContextMenu cm2;        
        private System.Windows.Forms.MenuItem menuCopy2;     
   //     private System.Windows.Forms.MenuItem menuPaste2;   
        // ContextMenu (IME, extended menu)
        private System.Windows.Forms.ContextMenu cm3;       
         
#if !EDICT
        private System.Windows.Forms.MenuItem menu_hangul_button;     
        private System.Windows.Forms.MenuItem menu_hiragana_button;     
        private System.Windows.Forms.MenuItem menu_hangul_keyboard;     
#endif
        private System.Windows.Forms.MenuItem menu_extended;  
        private System.Windows.Forms.MenuItem menuSeparator;
        private System.Windows.Forms.MenuItem menu_about;     
        private System.Windows.Forms.MenuItem menu_close;     

        private const int wMargin  = 3;

        private const int radioButtonWidth   = 75;
        private const int radioButtonWidth_s = 55;

        private const int inputAreaWidth  = radioButtonWidth * 4;
        private const int inputAreaHight  = 25;

        private const int radioButtonX1 = wMargin;
        private const int radioButtonX2 = wMargin + radioButtonWidth;
        private const int radioButtonX3 = wMargin + (radioButtonWidth * 2);
        private const int radioButtonX4 = wMargin + (radioButtonWidth * 3);
        private const int radioButtonY  = wMargin + inputAreaHight + wMargin ; 
        private const int radioButtonHight = inputAreaHight;
        
        
        // for 辞書切り替え

        private const int outputAreaY = radioButtonY +
                                        radioButtonHight + wMargin - 2; 
                                        // -2 is adjustment
                                        
        private const int outputAreaHight = 355;

        private const int formWidth = (wMargin * 2) + inputAreaWidth ;
        private const int formHight = outputAreaY 
                                         + outputAreaHight
                                         + wMargin;

        // コンパクト・フォーム用
        private const int inputAreaWidth_s  = radioButtonWidth_s * 4;
        private const int outputAreaHight_s = 270;
        private const int formWidth_s = (wMargin * 2) + inputAreaWidth_s ;
        private const int formHight_s = outputAreaY
                                        + outputAreaHight_s
                                        + wMargin ;
                             
        // KJ_formの設定を保持するオブジェクト
        public KJ_form_Setting Setting;
        
        private System.ComponentModel.Container components = null;

        public string      cultureName  ;
        
        // 検索絞込み用のフィルタ
        private FilterDelegate filter;
        
      //  private CharCategory inputCharCategory;  // 入力文字種別
        private bool inputIsHangul;    // 入力文字列はハングルか？
        private string searchingLabel; // 検索中
    
        
        // for Clipboard
        private IntPtr NextHandle; 
        [DllImport("user32")] 
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer); 
        [DllImport("user32", CharSet=CharSet.Auto)] 
        public extern static int SendMessage(IntPtr hWnd, int Msg, 
                                             IntPtr wParam, IntPtr lParam); 
        private const int WM_DRAWCLIPBOARD = 0x0308; 
        private const int WM_CHANGECBCHAIN = 0x030D; 

        // コンストラクタ
        public KJ_form()
        {
            msg = new KJ_Message();

            
            // 設定情報
            Setting = new KJ_form_Setting();
            
            // もし存在するならば設定をファイルから読み込む
            
            if (System.IO.File.Exists(Setting.SettingFileName) == true)
            {
                // 設定ファイルあり
                
                //XmlSerializerオブジェクトの作成
                XmlSerializer serializer2 =  
                    new XmlSerializer(typeof(KJ_form_Setting));
                //ファイルを開く
                FileStream fs2 = new FileStream(Setting.SettingFileName, 
                                                FileMode.Open,
                                                FileAccess.Read);
                //XMLファイルから読み込み、逆シリアル化する
                Setting  =  (KJ_form_Setting) serializer2.Deserialize(fs2);
                                
                // もし設定ファイルにカルチャ情報があれば，KJ_Messageに設定する
                //  (ふるい設定ファイルにはカルチャ情報はない)
                if(Setting.CultureName != null){
                    KJ_Message.SetCultureName(Setting.CultureName);
                }else{
                    // 無いならOSのデフォルトを設定
                    Setting.CultureName = KJ_Message.GetCultureName();
                }
                
                //閉じる
                fs2.Close();
            }else{
                // 設定ファイルがない時のdefault
                Setting.withPronunciation = false;
                Setting.PronunciationType = 1;
                Setting.TargetLang        = 1;
                Setting.CultureName       = KJ_Message.GetCultureName();
                Setting.debugInfo         = false;
                Setting.except9999        = false;
                Setting.except8888        = true;
                Setting.ClipboardView     = false;
#if  !EDICT
                Setting.CompactForm       = false;
#else
                Setting.CompactForm       = true;
#endif
            }
            
            
            // フォントを設定する
            FontSetting();
            
            
            // FormのInitialize
            InitializeComponent();
            
        
            //コンパクトモード切替
            if(this.Setting.CompactForm){
                this.ChangeCompactForm();
            }else{
                // Fromサイズを覚えていたらSettingから戻す
                if( Setting.FormSize.Width != 0 ){
                    this.ClientSize = Setting.FormSize ;
                }
            }
            
            // フィルタのデリゲート生成
            this.filter = new FilterDelegate(this.defaultFilter);
            
         // 韓国語環境テストが簡単にできるように関数化
            this.cultureName = KJ_Message.GetCultureName()  ;  
                               //  "ja-JP"  or ...
                        
            //  Open dictionary
      #if  EDICT
      //     KJ_dict.DictOpen("edict.yml");
           if(this.Setting.TargetLang == 1) {
               KJ_dict.DictOpen("edict.yml");
           }
           if(this.Setting.TargetLang == 2) {
               KJ_dict.DictOpen("edict.yml", "edict.yml.en");
           }
           if(this.Setting.TargetLang == 3) {
               KJ_dict.DictOpen("edict.yml", "edict.yml.jp");
           }
      #else      
           if(this.Setting.TargetLang == 1) {
               KJ_dict.DictOpen("KJ_dict.yml");
           }
           if(this.Setting.TargetLang == 2) {
               KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.kr");
           }
           if(this.Setting.TargetLang == 3) {
               KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.jp");
           }
      #endif    
            
            KJ_dict.SetFilter(this.filter);
            
            
            // for Clipboard
            NextHandle = SetClipboardViewer(this.Handle); 
            
        } // end of KJ_form

        
        //---------------------------------------------------------------------
        //
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        //---------------------------------------------------------------------
        public void FontSetting()
        {
            System.OperatingSystem os = System.Environment.OSVersion;

            // 検索文字入力，検索結果表示で使うフォント
            if(Setting.font != null && Setting.font != "" ){
                FontConverter fontConverter = new FontConverter();
                this.formFont = (Font)fontConverter.ConvertFromString(Setting.font);
            }else{
                
                //  フォントのカスタマイズがない場合
                if(os.Platform==PlatformID.Win32NT   &&
                   os.Version.Major >= 6){  // Vista以降
                    // Vista以降ならfontをメイリオにしてみる
                    this.formFont  = new System.Drawing.Font("メイリオ", 10F, 
                                               System.Drawing.FontStyle.Regular, 
                                               System.Drawing.GraphicsUnit.Point, 
                                               ((byte)(128)));
                }else{
                    // for Windows XP
                    this.formFont  = new System.Drawing.Font("", 10F, 
                                               System.Drawing.FontStyle.Regular, 
                                               System.Drawing.GraphicsUnit.Point, 
                                               ((byte)(128)));
                }
            }
            
            // 設定画面で使う，小さい方のフォントはカスタマイズさせない
            if(os.Platform==PlatformID.Win32NT   &&
               os.Version.Major >= 6){  // Vista以降
                // Vista以降ならfontをメイリオにしてみる
                this.formFont_s = new System.Drawing.Font("メイリオ", 9F, 
                                           System.Drawing.FontStyle.Regular, 
                                           System.Drawing.GraphicsUnit.Point, 
                                           ((byte)(128)));
            }else{
                // for Windows XP
                this.formFont_s = new System.Drawing.Font("", 9F, 
                                           System.Drawing.FontStyle.Regular, 
                                           System.Drawing.GraphicsUnit.Point, 
                                           ((byte)(128)));
            }
        
        }

        //---------------------------------------------------------------------
        private void InitializeComponent()
        {
            this.translatorThread = null;

            this.inputArea  = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();


      //      this.richTextBox2 = new RichTextBoxLinks.RichTextBoxEx();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();


            this.SuspendLayout();

            //================================================================ 
            // コンテキストメニュー用

            
#if !EDICT
            // コンテキストメニューの元ネタを作成
            
            // menu_hangul_button
            this.menu_hangul_button    = new System.Windows.Forms.MenuItem(); 
            this.menu_hangul_button.Click += new System.EventHandler
                                                (this.hangul_button_Click);
            // menu_hangul_keyboard
            this.menu_hangul_keyboard    = new System.Windows.Forms.MenuItem(); 
            this.menu_hangul_keyboard.Click += new System.EventHandler
                                                (this.hangul_keyboard_Click);
            // menu_hiragana_button
            this.menu_hiragana_button  = new System.Windows.Forms.MenuItem(); 
            this.menu_hiragana_button.Click += new System.EventHandler
                                                (this.hiragana_button_Click);
#endif

            // menu_extended
            this.menu_extended      = new System.Windows.Forms.MenuItem(); 
            this.menu_extended.Click += new System.EventHandler
                                                (this.extended_menu_Click);

            // menu_about
            this.menu_about         = new System.Windows.Forms.MenuItem(); 
            this.menu_about.Click += new System.EventHandler
                                                (this.menu_about_Click);
            menuSeparator = new MenuItem("-");
            // menu_close
            this.menu_close        = new System.Windows.Forms.MenuItem(); 
            this.menu_close.Click += new System.EventHandler
                                                (this.exitButton_Click);
                // 終了ボタンをやめコンテキストメニューにする

            //------------------------------------------------------------
            // 入力エリア用コンテキストメニュー

            // menuCopy
            this.menuCopy1  = new System.Windows.Forms.MenuItem();
            this.menuCopy1.Index = 1;
            this.menuCopy1.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuCopy1.Click += new System.EventHandler(this.menuCopy_Click1);
            // menuPaste
            this.menuPaste1 = new System.Windows.Forms.MenuItem();
            this.menuPaste1.Index = 2;
            this.menuPaste1.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuPaste1.Click += new System.EventHandler(this.menuPaste_Click1);
       
            this.cm1 = new ContextMenu();
            this.cm1.MenuItems.Add(this.menuCopy1);
            this.cm1.MenuItems.Add(this.menuPaste1);
            this.cm1.MenuItems.Add(this.menuSeparator.CloneMenu());
#if !EDICT
            this.cm1.MenuItems.Add(this.menu_hangul_button.CloneMenu());
            this.cm1.MenuItems.Add(this.menu_hangul_keyboard.CloneMenu());
            this.cm1.MenuItems.Add(this.menu_hiragana_button.CloneMenu());
#endif
            this.cm1.MenuItems.Add(this.menu_extended.CloneMenu());
            this.cm1.MenuItems.Add(this.menu_about.CloneMenu());
            this.cm1.MenuItems.Add(this.menu_close.CloneMenu());
           
            //-------------------------------------------------------
            // 出力エリア用コンテキストメニュー

            // menuCopy
            this.menuCopy2 = this.menuCopy1.CloneMenu();
            this.menuCopy2.Click += new System.EventHandler(this.menuCopy_Click2);

            this.cm2 = new ContextMenu();
            this.cm2.MenuItems.Add(this.menuCopy2);
            this.cm2.MenuItems.Add(this.menuSeparator.CloneMenu());
#if !EDICT
            this.cm2.MenuItems.Add(this.menu_hangul_button.CloneMenu());
            this.cm2.MenuItems.Add(this.menu_hangul_keyboard.CloneMenu());
            this.cm2.MenuItems.Add(this.menu_hiragana_button.CloneMenu());
#endif
            this.cm2.MenuItems.Add(this.menu_extended.CloneMenu());
            this.cm2.MenuItems.Add(this.menu_about.CloneMenu());
            this.cm2.MenuItems.Add(this.menu_close.CloneMenu());

            //------------------------------------------------------------
            // 基本部分用(全体フォーム:ボタン上に利く)
            this.cm3 = new ContextMenu();
#if !EDICT
            this.cm3.MenuItems.Add(this.menu_hangul_button);
            this.cm3.MenuItems.Add(this.menu_hangul_keyboard);
            this.cm3.MenuItems.Add(this.menu_hiragana_button);
#endif       
            this.cm3.MenuItems.Add(this.menu_extended);
            this.cm3.MenuItems.Add(this.menu_about);
            this.cm3.MenuItems.Add(this.menu_close);
            
            //================================================================ 


            //-----------------------------------------------------
            // 
            // inputArea
            // 
            this.inputArea.Location = new System.Drawing.Point(wMargin, wMargin);
            this.inputArea.Name = "inputArea";
            this.inputArea.Size = new System.Drawing.Size(inputAreaWidth, 
                                                          inputAreaHight);
            this.inputArea.TabIndex = 0;
            this.inputArea.Multiline = false;
            this.inputArea.Text = "";
            this.inputArea.Font = formFont; // [2009/05/13 23:02:19]
            this.inputArea.ContextMenu = this.cm1;
            this.inputArea.TextChanged  += 
                            new System.EventHandler(this.Text_Changed);
       //     this.inputArea.MouseHover += 
       //                new System.EventHandler(this.inputArea_MouseHover);
            this.inputArea.Anchor = ( AnchorStyles.Right | 
                                      AnchorStyles.Left  | AnchorStyles.Top); 


            // 
            // radioButton1    full
            // 
            this.radioButton1.Location = 
                  new System.Drawing.Point(radioButtonX1, radioButtonY);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = 
                       new System.Drawing.Size(radioButtonWidth, radioButtonHight);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.TabStop = true;
            this.radioButton1.Click += 
                       new System.EventHandler(this.radioButton_Click);
            this.radioButton1.MouseHover += 
                       new System.EventHandler(this.radioButton1_MouseHover);
            this.radioButton1.TextAlign  = ContentAlignment.MiddleCenter  ;  
            this.radioButton1.Font = formFont_s; 
            this.radioButton1.Appearance  = Appearance.Button ; 
            // 
            // radioButton2    forward
            // 
            this.radioButton2.Checked = true;
            this.radioButton2.Location = 
                      new System.Drawing.Point(radioButtonX2, radioButtonY);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = 
                      new System.Drawing.Size(radioButtonWidth, radioButtonHight);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Click += 
                         new System.EventHandler(this.radioButton_Click);
            this.radioButton2.MouseHover += 
                         new System.EventHandler(this.radioButton2_MouseHover);
            this.radioButton2.TextAlign  = ContentAlignment.MiddleCenter  ;  
            this.radioButton2.Font = formFont_s; 
            this.radioButton2.Appearance  = Appearance.Button ; 
            // 
            // radioButton3    backward
            // 
            this.radioButton3.Location = 
                      new System.Drawing.Point(radioButtonX3, radioButtonY);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size =
                       new System.Drawing.Size(radioButtonWidth, radioButtonHight);
            this.radioButton3.TabIndex = 5;
            this.radioButton3.Click += 
                          new System.EventHandler(this.radioButton_Click);
            this.radioButton3.MouseHover += 
                         new System.EventHandler(this.radioButton3_MouseHover);
            this.radioButton3.TextAlign  = ContentAlignment.MiddleCenter  ;  
            this.radioButton3.Font = formFont_s; 
            this.radioButton3.Appearance  = Appearance.Button ; 
            // 
            // radioButton4     part
            // 
            this.radioButton4.Location = 
                      new System.Drawing.Point(radioButtonX4, radioButtonY);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = 
                        new System.Drawing.Size(radioButtonWidth, radioButtonHight);
            this.radioButton4.TabIndex = 6;
            this.radioButton4.Click += 
                        new System.EventHandler(this.radioButton_Click);
            this.radioButton4.MouseHover += 
                         new System.EventHandler(this.radioButton4_MouseHover);
            this.radioButton4.TextAlign  = ContentAlignment.MiddleCenter  ;  
            this.radioButton4.Font = formFont_s; 
            this.radioButton4.Appearance  = Appearance.Button ; 


            
            // 
            // richTextBox2 （結果の表示エリア）
            // 
            this.richTextBox2.Location = 
                new System.Drawing.Point(wMargin, outputAreaY);
            this.richTextBox2.Size = new System.Drawing.Size(inputAreaWidth, 
                                                             outputAreaHight);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.TabIndex = 11;
            this.richTextBox2.Font = formFont; 
            this.richTextBox2.Text = "";
            this.richTextBox2.Anchor = ( AnchorStyles.Right  | AnchorStyles.Left | 
                                         AnchorStyles.Bottom | AnchorStyles.Top );
            this.richTextBox2.ContextMenu = this.cm2;
            this.richTextBox2.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            this.SetStyle(ControlStyles.Selectable, false);
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox2.LinkClicked += 
                  new System.Windows.Forms.LinkClickedEventHandler(
                         this.richTextBox2_LinkClicked);
     //       this.richTextBox2.MouseHover += 
     //                  new System.EventHandler(this.richTextBox2_MouseHover);
            
            //キーイベントをフォームで受け取る
            this.KeyPreview = true; 
            this.KeyPress += new KeyPressEventHandler(this.Form_KeyPress);
            this.KeyDown  += new KeyEventHandler(this.Form_KeyDown);

            // 
            // KJ_form
            // 
            this.ContextMenu = this.cm3;  
            this.Resize += new System.EventHandler(this.KJ_dictform_Resize);;  
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(formWidth, formHight);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                         this.radioButton1,
                                                         this.radioButton2,
                                                         this.radioButton3,
                                                         this.radioButton4,   
                                                         this.richTextBox2,
                                                         this.inputArea,
                                            //             this.exitButton,
                                                                      });
#if !EDICT 
            this.Name = "KJ_dictform";
            this.Text = "KJ_dictform";
#else
            this.Name = "KJ_Edict";
            this.Text = "KJ_Edict";

            // スタイル(Windowsバーの最小化・最大化・×を出さない)
       //     this.ControlBox = false;
       //     this.MaximizeBox = false;
#endif


            
            // ウィンドの閉じる[X]が押されたときのイベント登録
            this.FormClosing += 
                   new FormClosingEventHandler(this.KJ_form_FormClosing);
            
            this.ResumeLayout(false);
            
            //----------------------------------------------------------
            // ラベルに文字を入れる
            this.RefreshFormLabel(); 

        } // end of InitializeComponent


        [STAThread]
        static void Main() 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new KJ_form());
        }

        //--------------------------------------------------------------------
        // クリップボードの監視
        protected override void WndProc(ref Message msg) { 
            switch (msg.Msg) { 
            case WM_DRAWCLIPBOARD: 
                // クリップボード翻訳onの時
                if(this.Setting.ClipboardView){
                    this.inputArea.Text = 
                        (string)Clipboard.GetDataObject().GetData(
                                                   DataFormats.UnicodeText ); 
                    if((int)NextHandle != 0) {
                        SendMessage(NextHandle, msg.Msg, msg.WParam, msg.LParam); 
                    }
                }
                break; 
            case WM_CHANGECBCHAIN: 
                NextHandle = (IntPtr)msg.LParam; 
                if((int)NextHandle != 0) {
                    SendMessage(NextHandle, msg.Msg, msg.WParam, msg.LParam); 
                }
                break; 
            } 
            
            base.WndProc(ref msg); 
        }

        //--------------------------------------------------------------------
        // ラジオボタンのクリックか入力エリアの変化があったら辞書引き開始
        private void radioButton_Click(object sender, System.EventArgs e)
        {
            form_search(  );
        }
        // 辞書切り替えボタンのクリック
        private void radioButton2_Click(object sender, System.EventArgs e)
        {
           // form_search(  );
        }
        //--------------------------------------------------------------------
        // マウスホバーで選択させる
        private void radioButton1_MouseHover(object sender, System.EventArgs e)
        {
            this.radioButton1.Checked = true;
            form_search(  );
        }
        private void radioButton2_MouseHover(object sender, System.EventArgs e)
        {
            this.radioButton2.Checked = true;
            form_search(  );
        }
        private void radioButton3_MouseHover(object sender, System.EventArgs e)
        {
            this.radioButton3.Checked = true;
            form_search(  );
        }
        private void radioButton4_MouseHover(object sender, System.EventArgs e)
        {
            this.radioButton4.Checked = true;
            form_search(  );
        }

        //--------------------------------------------------------------------
    //    // マウスホバーで入出力エリアにフォーカスをあてる
    //    private void richTextBox2_MouseHover(object sender, System.EventArgs e)
    //    {
    //        this.richTextBox2.Focus(); 
    //    }
    //    private void inputArea_MouseHover(object sender, System.EventArgs e)
    //    {
    //        this.inputArea.Focus(); 
    //    }
    //
    //     [2011/07/01 22:12:34]
    
        //---------------------------------------------------------------------
        private void Text_Changed(object sender, System.EventArgs e)
        {
            form_search();
        }
        
        
        //---------------------------------------------------------------------
        // プログラム終了
        private void exitButton_Click(object sender, System.EventArgs e)
        {
            // 終了時に画面サイズを記憶する
            SaveClientSize();

            // 設定を保存
            this.Setting.Save();
            
            // フォーム終了
            this.Close();
        }

        //---------------------------------------------------------------------
        // Formの閉じるボタン（右上の[X]）が押された場合。
        private void KJ_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 終了時に画面サイズを記憶する
            SaveClientSize();

            // 設定を保存
            this.Setting.Save();
            
        }
        
        // フォームサイズを記憶する
        public void SaveClientSize()
        {
            if(Setting.CompactForm){
                Setting.CompactFormSize = this.ClientSize   ;
            }else{
                Setting.FormSize = this.ClientSize   ;
            }
        }
        
        //=======================================================================
        private void menuCopy_Click1(object sender, System.EventArgs e)
        {
            if(this.inputArea.SelectionLength > 0){
                this.inputArea.Copy();       
            }
        }
        private void menuPaste_Click1(object sender, System.EventArgs e)
        {
            if(Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                this.inputArea.Paste();      
            }
        }
        //---------------------------------------------------------------------
        private void menuCopy_Click2(object sender, System.EventArgs e)
        {
            if(this.richTextBox2.SelectionLength > 0) {
                this.richTextBox2.Copy();       
            }
        }
        private void menuPaste_Click2(object sender, System.EventArgs e)
        {
            if(Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                this.richTextBox2.Paste();      
            }
        }
        

#if !EDICT
        //------------------------------------------------------------------------
        // 押された特殊キーを拾う
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // エスケープキーが押されたら入力エリアをクリア
            if (e.KeyCode == Keys.Escape){
                if(hangul_keyboard_instance == null){
                    this.inputArea.Text = "";
                    return;
                }else{
                    // ハングルキーボードが出ているが本体側にフォーカスがある場合
                    this.inputKey = "";
                    this.inputArea.Text = "";
                    return;
                }
            }

            // Hangul keyboard未表示
            if(hangul_keyboard_instance == null){
                
                // バックスペースキー
                if(e.KeyCode == Keys.Back && this.inputArea.Text.Length > 0){
                    if(this.inputArea.Focused == true){
                        // NOP
                    }else{
                        // 入力エリアにフォーカスがなくともBackSpaceを利かせる
                        this.inputArea.Text = 
                              this.inputArea.Text.Remove(
                                       this.inputArea.Text.Length - 1);         
                        
                    }
                }
                return; // keyboard未表示時はかならずここで抜ける
            }
            
            // Hangul keyboardが表示されており，
            // backSpaceが押され，1文字以上残っている場合，1キー分戻す
            if (e.KeyCode == Keys.Back && inputKey.Length > 0)
            {
                inputKey = inputKey.Remove(inputKey.Length - 1);
   
                this.inputArea.Text =  HYAM.Lingua.Hangul.KeyString2Hangul(inputKey); 
            }
        }
        //----------------------------------------------------------------------
        // 押されたキーを拾う
        //   （KeyPressは特殊キーは拾えない）
        private void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if(hangul_keyboard_instance == null){  
                // Hangul keyboard未表示
                if (CodeCheck.IsAlpah(e.KeyChar))
                {
                    if(this.inputArea.Focused == true){
                        // NOP
                    }else{
                        // 入力エリアにフォーカスがなくとも英数字は
                        // 入力可能とする
                        this.inputArea.Text += e.KeyChar;
                    }
                }
                
                return; // Hangul keyboard未表示時はかならずここで抜ける
            }
            
            // Hangul keyboardが表示されていて，かつ本体側にフォーカスがある場合
            // キーを拾う処理
            if (CodeCheck.IsAlpah(e.KeyChar))
            {
                inputKey += e.KeyChar;
            }else{
                return;
            }
            this.inputArea.Text = HYAM.Lingua.Hangul.KeyString2Hangul(inputKey); ;
        }
#else
    // for Edict
        //------------------------------------------------------------------------
        // 押された特殊キーを拾う
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // エスケープキーが押されたら入力エリアをクリア
            if (e.KeyCode == Keys.Escape){
                this.inputArea.Text = "";
                return;
            }

            // バックスペースキー
            if(e.KeyCode == Keys.Back && this.inputArea.Text.Length > 0){
                if(this.inputArea.Focused == true){
                    // NOP
                }else{
                    // 入力エリアにフォーカスがなくともBackSpaceを利かせる
                    this.inputArea.Text = 
                          this.inputArea.Text.Remove(
                                   this.inputArea.Text.Length - 1);         
                    
                }
            }
            
        }
        //----------------------------------------------------------------------
        // 押されたキーを拾う
        //   （KeyPressは特殊キーは拾えない）
        private void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CodeCheck.IsAlpah(e.KeyChar))
            {
                if(this.inputArea.Focused == true){
                    // NOP (普通に入力できるため，ここでは何もしない)
                }else{
                    // 入力エリアにフォーカスがなくとも英数字は
                    // 入力可能とする
                    this.inputArea.Text += e.KeyChar;
                }
            }
        }
    
#endif

#if !EDICT
        //------------------------------------------------------------------------
        // Panel上のコンテキストメニュー
        private void hangul_button_Click(object sender, System.EventArgs e)
        {
           // モードレス
           if(hangul_button_instance == null || 
              hangul_button_instance.IsDisposed) {
               hangul_button_instance = new Hangul_Button_Form(this);

               // 要素検索に変更
               KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.krparts");

               // 表示するIMEに現在のKJ_formを設定
               hangul_button_instance.kj_form = this ;

               hangul_button_instance.Show();
            }else{
                if( hangul_button_instance != null){
                    hangul_button_instance.Focus() ;
                }
           }
        }
        
        private void hiragana_button_Click(object sender, System.EventArgs e)
        {
           // モードレス
           if(hiragana_button_instance == null || 
              hiragana_button_instance.IsDisposed) {
               hiragana_button_instance = new Hiragana_Button_Form(this);

               hiragana_button_instance.Show();
            }else{
                if( hiragana_button_instance != null){
                    hiragana_button_instance.Focus() ;
                }
           }
        }

        protected void hangul_keyboard_Click(object sender, System.EventArgs e)
        {
           // モードレス
           if(hangul_keyboard_instance == null ||
              hangul_keyboard_instance.IsDisposed) {
               hangul_keyboard_instance = new Hangul_keyboard_Form(this);

               hangul_keyboard_instance.Show();
            }else{
                if( hangul_keyboard_instance != null){
                    hangul_keyboard_instance.Focus() ;
                }
           }
           
           // IME優先にするため入力抑止
           this.inputArea.ReadOnly = true;  //[2009/08/16 23:55:40]
        }

#endif

        protected void menu_about_Click(object sender, System.EventArgs e)
        {
           // モードレス
           if(about_menu_instance == null || about_menu_instance.IsDisposed) {
               about_menu_instance = new About_Form();

               about_menu_instance.Show();

            }else{
                if( about_menu_instance != null){
                    about_menu_instance.Focus() ;
                }
           }
        }

        
        //---------------------------------------------------------------------
        // 拡張メニューを開く
        private void extended_menu_Click(object sender, System.EventArgs e)
        {
           // モードレス
           if(extended_menu_instance == null || 
              extended_menu_instance.IsDisposed) {
               extended_menu_instance = new Extended_menu_Form(this);
            
               // 初期状態をSetting情報に変える
#if !EDICT
               extended_menu_instance.checkBox1.Checked = 
                                        this.Setting.withPronunciation;
               extended_menu_instance.checkBox2.Checked = this.Setting.debugInfo;
               extended_menu_instance.checkBox3.Checked = this.Setting.except9999; 
               extended_menu_instance.checkBox4.Checked = this.Setting.except8888; 
#endif               

               extended_menu_instance.checkBox5.Checked 
                                                 = this.Setting.ClipboardView; 

#if !EDICT
               extended_menu_instance.checkBox6.Checked = this.Setting.CompactForm; 


               if(this.Setting.PronunciationType == 1) {
                   extended_menu_instance.radio_kana.Checked = true;
               }
               if(this.Setting.PronunciationType == 2) {
                   extended_menu_instance.radio_alpha.Checked = true;
               }
#endif               

               if(this.Setting.TargetLang == 1) {
                   extended_menu_instance.radio_all.Checked = true;
               }
               if(this.Setting.TargetLang == 2) {
                   extended_menu_instance.radio_kr.Checked = true;
               }
               if(this.Setting.TargetLang == 3) {
                   extended_menu_instance.radio_jp.Checked = true;
               }

#if !EDICT
               // ラベル表示に使う言語
               if(this.Setting.CultureName == "en-US") {
                   extended_menu_instance.radio_culture_en.Checked = true;
               }
               if(this.Setting.CultureName == "ko-KR") {
                   extended_menu_instance.radio_culture_kr.Checked = true;
               }
               if(this.Setting.CultureName == "ja-JP") {
                   extended_menu_instance.radio_culture_jp.Checked = true;
               }
               // withPronunciationがfalseなら不活性に
               if(this.Setting.withPronunciation){
                   extended_menu_instance.radio_kana.Enabled  = true;
                   extended_menu_instance.radio_alpha.Enabled = true;
               }else{
                   extended_menu_instance.radio_kana.Enabled  = false;
                   extended_menu_instance.radio_alpha.Enabled = false;
               }
#endif               
            
               extended_menu_instance.Show();
            }else{
                if( extended_menu_instance != null){
                    extended_menu_instance.Focus() ;
                }
            }
        }

        
        //------------------------------------------------------------------------
        public void RefreshFormLabel()
        {

            this.menuCopy1.Text  = msg.Get("L079") + " (&C)"; // Copy
            this.menuPaste1.Text = msg.Get("L080") + " (&P)"; // Paste
            this.menu_close.Text = msg.Get("L005") ; // 終了;  
            this.menu_about.Text = msg.Get("L081") ; // about;  
#if !EDICT
            this.menu_extended.Text = msg.Get("L007") ;  
            this.menu_hangul_button.Text   = msg.Get("L063"); // Hangul IME
            this.menu_hangul_keyboard.Text = msg.Get("L064"); // Hangul Keyboard
            this.menu_hiragana_button.Text = msg.Get("L065"); // Hiragana IME

            // 入力エリアコンテキストメニュー用(cm1)
            this.cm1.MenuItems[3].Text = msg.Get("L063"); // Hangul IME   
            this.cm1.MenuItems[4].Text = msg.Get("L064"); // Hangul Keyboard
            this.cm1.MenuItems[5].Text = msg.Get("L065"); // Hiragana IME
            this.cm1.MenuItems[6].Text = msg.Get("L007") ; //拡張メニュー 
            this.cm1.MenuItems[7].Text = msg.Get("L081") ; // about;   
            this.cm1.MenuItems[8].Text = msg.Get("L005") ; // 終了;   
            
            // 出力エリアコンテキストメニュー用(cm2)
            this.cm2.MenuItems[0].Text = msg.Get("L079") + " (&C)"; // Copy   
            this.cm2.MenuItems[2].Text = msg.Get("L063"); // Hangul IME   
            this.cm2.MenuItems[3].Text = msg.Get("L064"); // Hangul Keyboard
            this.cm2.MenuItems[4].Text = msg.Get("L065"); // Hiragana IME
            this.cm2.MenuItems[5].Text = msg.Get("L007") ; //拡張メニュー 
            this.cm2.MenuItems[6].Text = msg.Get("L081") ; // about;  
            this.cm2.MenuItems[7].Text = msg.Get("L005") ; // 終了;  
#else       
            // Edict     
            this.cm1.MenuItems[0].Text = msg.Get("L079") + " (&C)"; // Copy   
            this.cm1.MenuItems[1].Text = msg.Get("L080") + " (&P)"; // Paste   
       //   this.cm1.MenuItems[2]     セパレータ
            this.cm1.MenuItems[3].Text = msg.Get("L007") ; // 拡張メニュー ;  
            this.cm1.MenuItems[4].Text = msg.Get("L084") ; // about;   
            this.cm1.MenuItems[5].Text = msg.Get("L005") ; // 終了   
            
            this.cm2.MenuItems[0].Text = msg.Get("L079") + " (&C)"; // Copy   
       //   this.cm2.MenuItems[1]     セパレータ   
            this.cm2.MenuItems[2].Text = msg.Get("L007") ; // 拡張メニュー ;  
            this.cm2.MenuItems[3].Text = msg.Get("L084") ; // about; 
            this.cm2.MenuItems[4].Text = msg.Get("L005") ; // 終了   

            this.cm3.MenuItems[0].Text = msg.Get("L007") ; // 拡張メニュー ;  
            this.cm3.MenuItems[1].Text = msg.Get("L084") ; // about; 
            this.cm3.MenuItems[2].Text = msg.Get("L005") ; // 終了   
#endif                                   

       //     this.exitButton.Text = msg.Get("L005") ;  // Exit
            
            if(this.Setting.CompactForm){
                // コンパクトモード
                this.radioButton1.Text = msg.Get("L034") ;  
                this.radioButton2.Text = msg.Get("L035") ;  
                this.radioButton3.Text = msg.Get("L036") ;  
                this.radioButton4.Text = msg.Get("L037") ;  
            }else{
                // 通常サイズ
                this.radioButton1.Text = msg.Get("L001") ;  
                this.radioButton2.Text = msg.Get("L002") ;  
                this.radioButton3.Text = msg.Get("L003") ;  
                this.radioButton4.Text = msg.Get("L004") ;  
            }

            this.searchingLabel = msg.Get("L025");
        }

        //=======================================================================
        // コンパクトモード切替
        public void ChangeCompactForm(  ){
            this.ClientSize = new System.Drawing.Size(formWidth_s, 
                                                      formHight_s);

          //  this.exitButton.Visible=false;

            this.radioButton1.Text = msg.Get("L034") ;  
            this.radioButton2.Text = msg.Get("L035") ;  
            this.radioButton3.Text = msg.Get("L036") ;  
            this.radioButton4.Text = msg.Get("L037") ;  
            
            this.richTextBox2.Size = 
                new System.Drawing.Size(inputAreaWidth_s, 
                                        outputAreaHight_s);
            this.richTextBox2.Location = 
                new System.Drawing.Point(wMargin, outputAreaY);

         //   this.inputArea.Location = new System.Drawing.Point(wMargin, wMargin);

            
            // もしあればFromサイズを記憶から戻す
            if( Setting.CompactFormSize.Width != 0 ){
                this.ClientSize = Setting.CompactFormSize ;
            }
            
            // スタイル
//            this.FormBorderStyle = 
//                System.Windows.Forms.FormBorderStyle.SizableToolWindow;
//アイコン化ボタンは欲しいのでスタイルは変更しない                
        }
        //-----------------------------------------------------------------
        // 通常モード切替 (通常サイズに戻す)
        public void ChangeNormalForm(  ){
            this.ClientSize = new System.Drawing.Size(formWidth, formHight);
           // this.exitButton.Visible=true;

            this.radioButton1.Text = msg.Get("L001") ;  
            this.radioButton2.Text = msg.Get("L002") ;  
            this.radioButton3.Text = msg.Get("L003") ;  
            this.radioButton4.Text = msg.Get("L004") ;  
            
            this.richTextBox2.Size = 
                      new System.Drawing.Size(inputAreaWidth, 
                                              outputAreaHight);
            this.richTextBox2.Location = 
                new System.Drawing.Point(wMargin, outputAreaY);

//            this.inputArea.Location = new System.Drawing.Point(wMargin, wMargin);

            // もしあればFromサイズを記憶から戻す
            if( Setting.FormSize.Width != 0 ){
                this.ClientSize = Setting.FormSize ;
            }
            
            // スタイル
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }
        
        //-----------------------------------------------------------------
        private void richTextBox2_LinkClicked(object sender, 
                        System.Windows.Forms.LinkClickedEventArgs e)
        {
            // リンクされたテキストを入力エリアに反映
            this.inputArea.Text = e.LinkText ;
        }
        //-----------------------------------------------------------------
        private void KJ_dictform_Resize(object sender, EventArgs e)
        {
            int buttonWidth = this.inputArea.Width / 4 ;

            this.radioButton1.Size = 
                  new System.Drawing.Size(buttonWidth, radioButtonHight);

            this.radioButton2.Size = 
                  new System.Drawing.Size(buttonWidth, radioButtonHight);
            this.radioButton2.Location = 
                  new System.Drawing.Point(radioButtonX1 + buttonWidth, 
                                           radioButtonY);

            this.radioButton3.Size = 
                  new System.Drawing.Size(buttonWidth, radioButtonHight);
            this.radioButton3.Location = 
                  new System.Drawing.Point(radioButtonX1 + buttonWidth*2, 
                                           radioButtonY);

            this.radioButton4.Size = 
                  new System.Drawing.Size(buttonWidth, radioButtonHight);
            this.radioButton4.Location = 
                  new System.Drawing.Point(radioButtonX1 + buttonWidth*3, 
                                           radioButtonY);

        }
        //-----------------------------------------------------------------

    } 
    // end of class KJ_form
    

}
