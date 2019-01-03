// Copyright 2004, 2010 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

using HYAM.Lingua;

namespace HYAM.KJ_dict
{

    public class Extended_menu_Form : System.Windows.Forms.Form
    {

        private System.Windows.Forms.Button exitButton;
        
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
#if !EDICT
        private System.Windows.Forms.GroupBox groupBox3;
        
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.CheckBox checkBox2;
        public System.Windows.Forms.CheckBox checkBox3;
        public System.Windows.Forms.CheckBox checkBox4;
#endif

        public System.Windows.Forms.CheckBox checkBox5;
#if !EDICT
        public System.Windows.Forms.CheckBox checkBox6;

        public System.Windows.Forms.RadioButton radio_kana;
        public System.Windows.Forms.RadioButton radio_alpha;


        public System.Windows.Forms.RadioButton radio_culture_en;
        public System.Windows.Forms.RadioButton radio_culture_kr;
        public System.Windows.Forms.RadioButton radio_culture_jp;
#endif
        public System.Windows.Forms.RadioButton radio_all;
        public System.Windows.Forms.RadioButton radio_kr;   // (*1)
        public System.Windows.Forms.RadioButton radio_jp;
        
        // (*1) KJ_Edictでは英語を示す

        private System.Windows.Forms.Button resetButton;

        private System.Windows.Forms.Button fontButton;
        private System.Windows.Forms.Button resetFontButton;
        
        // 本体側オブジェクト
        public KJ_form kj_form; 
        
        
        private static KJ_Message msg;


        // コンストラクタ
        public Extended_menu_Form(KJ_form parent)
        {
            this.kj_form = parent;
            
            msg = new KJ_Message();
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {

            const int buttonHeight  = 20;

            const int formMargin   = 10;
            const int grpMargin    = 30;
            
            const int mainFormWidth     = 300;
//#if !EDICT
            const int radioButtonSizeX  = 150;
//#endif
            
            const int checkBox1_x = 25;
            const int indent = 20;

            int grp_pos_y = 0;
            int grpWidth = mainFormWidth - (formMargin*2) ;

            int posY ;  // 最初の


            // 閉じるボタン
            this.exitButton = new System.Windows.Forms.Button();
            this.exitButton.Size     = new System.Drawing.Size(60, buttonHeight);
            posY = formMargin;
            this.exitButton.Location = new System.Drawing.Point( 
                                          mainFormWidth - 65, posY );
            this.exitButton.Click += new System.EventHandler(this.exit_Click);
            this.Controls.Add(this.exitButton); 

            
#if !EDICT
            // 
            // checkBox1 and radioButton
            // 
            grp_pos_y = 20; // 初期設定。groupBox1の中でのy軸な点に注意
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox1.Location = 
                new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(250, 20);
            this.checkBox1.CheckedChanged += 
                     new System.EventHandler(this.checkBox1_Check);
            
            grp_pos_y += 20;
            this.radio_kana = new System.Windows.Forms.RadioButton();
            this.radio_kana.Location = 
                    new System.Drawing.Point(checkBox1_x + indent, grp_pos_y);
            this.radio_kana.Name = "radio_kana";
            this.radio_kana.Size = new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_kana.Click += 
                new System.EventHandler(this.radioButton_Click);

            grp_pos_y += 20;
            this.radio_alpha = new System.Windows.Forms.RadioButton();
            this.radio_alpha.Location = 
                     new System.Drawing.Point(checkBox1_x + indent, grp_pos_y);
            this.radio_alpha.Name = "radio_alpha";
            this.radio_alpha.Size = new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_alpha.Click += 
                new System.EventHandler(this.radioButton_Click);

            // 
            // checkBox2
            // 
            grp_pos_y += 35;
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox2.Location = 
                    new System.Drawing.Point(checkBox1_x, grp_pos_y);
                                                               
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(200, 20);
            this.checkBox2.CheckedChanged += 
                     new System.EventHandler(this.checkBox2_Check);

            // 
            // checkBox3
            // 
            grp_pos_y += 30;
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox3.Location = 
                    new System.Drawing.Point(checkBox1_x,  grp_pos_y);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(230, 20);
            this.checkBox3.CheckedChanged += 
                        new System.EventHandler(this.checkBox3_Check);
              

            // 
            // checkBox4
            // 
            grp_pos_y += 30;
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox4.Location = 
                     new System.Drawing.Point(checkBox1_x,  grp_pos_y);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(230, 20);
            this.checkBox4.CheckedChanged += 
                     new System.EventHandler(this.checkBox4_Check);
#endif

            // 
            // checkBox5
            // 
            grp_pos_y += 30;
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox5.Location =
                    new System.Drawing.Point(checkBox1_x,  grp_pos_y);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(230, 20);
            this.checkBox5.CheckedChanged += 
                        new System.EventHandler(this.checkBox5_Check);

#if !EDICT
            // 
            // checkBox6
            // 
            grp_pos_y += 30;
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox6.Location = 
                    new System.Drawing.Point(checkBox1_x,  grp_pos_y);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(230, 20);
            this.checkBox6.CheckedChanged += 
                    new System.EventHandler(this.checkBox6_Check);
#endif


            // 
            // groupBox1
            // 
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.Controls.AddRange(
                             new System.Windows.Forms.Control[] {
#if !EDICT
                                                 this.checkBox1,
                                                 this.radio_kana , 
                                                 this.radio_alpha,
                                                 this.checkBox2,
                                                 this.checkBox3,
                                                 this.checkBox4,
                                                 this.checkBox5,
                                                 this.checkBox6
#else
                                                 this.checkBox5
#endif
                                            });
            posY = posY + buttonHeight + formMargin;
            this.groupBox1.Location = 
                    new System.Drawing.Point(formMargin, posY);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = 
                    new System.Drawing.Size(grpWidth, 
                                            grp_pos_y + grpMargin );
            this.groupBox1.TabStop = false;
            this.Controls.Add(this.groupBox1); 


            //----------------------------------------------------------
//#if !EDICT
            grp_pos_y = 15;  // 初期設定。groupBox2の中でのy軸な点に注意
            this.radio_all = new System.Windows.Forms.RadioButton();
            this.radio_all.Location = 
                new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_all.Name = "radio_all";
            this.radio_all.Size = new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_all.Click += 
                new System.EventHandler(this.selectTargetDictLang_Click);
            this.radio_all.Checked = true;  // デフォルト


            grp_pos_y += 25;
            this.radio_kr = new System.Windows.Forms.RadioButton();
            this.radio_kr.Location = 
                    new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_kr.Name = "radio_kr";
            this.radio_kr.Size = new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_kr.Click += 
                    new System.EventHandler(this.selectTargetDictLang_Click);

            grp_pos_y += 25;
            this.radio_jp = new System.Windows.Forms.RadioButton();
            this.radio_jp.Location = 
                    new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_jp.Name = "radio_jp";
            this.radio_jp.Size = new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_jp.Click += 
                    new System.EventHandler(this.selectTargetDictLang_Click);

            // 
            // groupBox2
            // 
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox2.Controls.AddRange(
                             new System.Windows.Forms.Control[] {
                                                 this.radio_all , 
                                                 this.radio_kr,
                                                 this.radio_jp
                                            });
            
            posY = posY + groupBox1.Height + formMargin;
            this.groupBox2.Location = 
                new System.Drawing.Point(formMargin, posY);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = 
               new System.Drawing.Size(grpWidth, grp_pos_y + grpMargin);
            this.groupBox2.TabStop = false;
            this.Controls.Add(this.groupBox2); 

#if !EDICT
            //-------------------------------------------------------------------
            grp_pos_y = 15;  // 初期設定。groupBox3の中でのy軸な点に注意
            this.radio_culture_en = new System.Windows.Forms.RadioButton();
            this.radio_culture_en.Location = 
               new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_culture_en.Name = "radio_culture_en";
            this.radio_culture_en.Size = 
                new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_culture_en.Click += 
                new System.EventHandler(this.selectFormLang_Click);

            grp_pos_y += 25;
            this.radio_culture_kr = new System.Windows.Forms.RadioButton();
            this.radio_culture_kr.Location = 
                new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_culture_kr.Name = "radio_culture_kr";
            this.radio_culture_kr.Size =
                new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_culture_kr.Click += 
                new System.EventHandler(this.selectFormLang_Click);

            grp_pos_y += 25;
            this.radio_culture_jp = new System.Windows.Forms.RadioButton();
            this.radio_culture_jp.Location = 
                new System.Drawing.Point(checkBox1_x, grp_pos_y);
            this.radio_culture_jp.Name = "radio_culture_jp";
            this.radio_culture_jp.Size = 
                new System.Drawing.Size(radioButtonSizeX, 24);
            this.radio_culture_jp.Click += 
                new System.EventHandler(this.selectFormLang_Click);
            
            // 
            // groupBox3
            // 
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox3.Controls.AddRange(
                             new System.Windows.Forms.Control[] {
                                                 this.radio_culture_en , 
                                                 this.radio_culture_kr,
                                                 this.radio_culture_jp
                                            });
            posY = posY + groupBox2.Height + formMargin;
            this.groupBox3.Location = 
                new System.Drawing.Point(formMargin, posY);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = 
                new System.Drawing.Size(grpWidth, grp_pos_y + grpMargin);
            this.groupBox3.TabStop = false;
            this.Controls.Add(this.groupBox3); 
#endif


            //----------------------------------------------------------
            // 
            // Reset Button to defalt
            // 
            this.resetButton = new System.Windows.Forms.Button();
            this.resetButton.Size     = 
                new System.Drawing.Size(200, buttonHeight);
            
            posY = posY + grp_pos_y + grpMargin + formMargin;
            this.resetButton.Location = 
                new System.Drawing.Point(formMargin + indent, 
                                         posY );
            this.resetButton.Click += new System.EventHandler(this.reset_Click);
            this.Controls.Add(this.resetButton); 
            
            //----------------------------------------------------------
            // 
            //  Button for FontDialog
            // 
            this.fontButton = new System.Windows.Forms.Button();
            this.fontButton.Size     = 
                new System.Drawing.Size(200, buttonHeight);
            posY = posY + buttonHeight + formMargin*3 ;
            this.fontButton.Location = 
                new System.Drawing.Point(formMargin + indent, posY);
            this.fontButton.Click += 
                      new System.EventHandler(this.fontdialog_Click);
            this.Controls.Add(this.fontButton); 

            //----------------------------------------------------------
            // 
            //  Button for Font Reset
            // 
            this.resetFontButton = new System.Windows.Forms.Button();
            this.resetFontButton.Size     = 
                new System.Drawing.Size(200, buttonHeight);
            posY = posY + buttonHeight + formMargin ;
            this.resetFontButton.Location = 
                new System.Drawing.Point(formMargin + indent, posY);
            this.resetFontButton.Click += 
                      new System.EventHandler(this.resetFontdialog_Click);
            this.Controls.Add(this.resetFontButton); 



            //----------------------------------------------------------
            // Form全体
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            posY = posY + buttonHeight + formMargin*2 ;
            this.ClientSize = new System.Drawing.Size(mainFormWidth, posY);
            this.Name = "KJ_form_extended_menu";
            this.Text = "KJ_form Extended Menu";
            
            //----------------------------------------------------------
            // ラベルに文字を入れる
            this.RefreshFormLabel(); 
            
            
            //----------------------------------------------------------
            // フォームの位置を調整可能にする
            this.StartPosition = FormStartPosition.Manual;
            // 位置設定
            this.Left = this.kj_form.Right;
            this.Top  = this.kj_form.Top  ;
            
        }  // end of InitializeComponent
        

        //------------------------------------------------------------------------
        private void exit_Click(object sender, System.EventArgs e)
        {
            // 設定を保存
            this.kj_form.Setting.Save();
            // フォーム終了
            this.Close();
        }
#if !EDICT
        //------------------------------------------------------------------------
        private void checkBox1_Check(object sender, System.EventArgs e)
        {   
            this.kj_form.Setting.withPronunciation = this.checkBox1.Checked;
            if(this.checkBox1.Checked){
                this.radio_kana.Enabled   = true ;
                this.radio_alpha.Enabled  = true;
            }else{
                this.radio_kana.Enabled   = false ;
                this.radio_alpha.Enabled  = false;
            }
        }
        //------------------------------------------------------------------------
        // cost=9999 FreeWnnの単漢字
        private void checkBox3_Check(object sender, System.EventArgs e)
        {   
            this.kj_form.Setting.except9999 = this.checkBox3.Checked;
        }
        //------------------------------------------------------------------------
        // cost=8888 Hada語幹の変化形
        private void checkBox4_Check(object sender, System.EventArgs e)
        {   
            this.kj_form.Setting.except8888 = this.checkBox4.Checked;
        }
        //------------------------------------------------------------------------
        // コンパクトモード切替
        private void checkBox6_Check(object sender, System.EventArgs e)
        {   
            // 切り替え前のフォームサイズを記憶する
            this.kj_form.SaveClientSize();
            
            this.kj_form.Setting.CompactForm = this.checkBox6.Checked;
            if(this.checkBox6.Checked){
                this.kj_form.ChangeCompactForm();
            }else{
                this.kj_form.ChangeNormalForm();
            }
        }
        //------------------------------------------------------------------------
        // 発音表示
        private void radioButton_Click(object sender, System.EventArgs e)
        {   
            if(this.radio_kana.Checked ){
                this.kj_form.Setting.PronunciationType = 1;
            }else{
                this.kj_form.Setting.PronunciationType = 2;
            }
        }

        //------------------------------------------------------------------------
        private void checkBox2_Check(object sender, System.EventArgs e)
        {   
            this.kj_form.Setting.debugInfo = this.checkBox2.Checked;
        }
#endif            
        
        //------------------------------------------------------------------------
        // Clipboard監視
        private void checkBox5_Check(object sender, System.EventArgs e)
        {   
            this.kj_form.Setting.ClipboardView = this.checkBox5.Checked;
        }

        
        //------------------------------------------------------------------------
        // 検索対象の言語(辞書)を切り替る
        private void selectTargetDictLang_Click(object sender, System.EventArgs e)
        {   
#if !EDICT
            if(this.radio_all.Checked ){
                KJ_dict.DictOpen("KJ_dict.yml");
                this.kj_form.Setting.TargetLang = 1;
            }
            if(this.radio_kr.Checked ){
                KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.kr");
                this.kj_form.Setting.TargetLang = 2;
            }
            if(this.radio_jp.Checked ){
                KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.jp");
                this.kj_form.Setting.TargetLang = 3;
            }
#else            
            if(this.radio_all.Checked ){
                KJ_dict.DictOpen("edict.yml");
                this.kj_form.Setting.TargetLang = 1;
            }
            if(this.radio_kr.Checked ){
                KJ_dict.DictOpen("edict.yml", "edict.yml.en");
                this.kj_form.Setting.TargetLang = 2;
            }
            if(this.radio_jp.Checked ){
                KJ_dict.DictOpen("edict.yml", "edict.yml.jp");
                this.kj_form.Setting.TargetLang = 3;
            }
#endif            

            this.kj_form.form_search(  );  // 再検索
        }


#if !EDICT
        //------------------------------------------------------------------------
        // 表示モードの言語を切り替る
        private void selectFormLang_Click(object sender, System.EventArgs e)
        {   
            if(this.radio_culture_en.Checked ){
                KJ_Message.SetCultureName("en-US");
                this.kj_form.Setting.CultureName = "en-US"; 
            }
            if(this.radio_culture_kr.Checked ){
                KJ_Message.SetCultureName("ko-KR");
                this.kj_form.Setting.CultureName = "ko-KR"; 
            }
            if(this.radio_culture_jp.Checked ){
                KJ_Message.SetCultureName("ja-JP");
                this.kj_form.Setting.CultureName = "ja-JP";
            }

            this.kj_form.cultureName = KJ_Message.GetCultureName()  ;  


            this.RefreshFormLabel();          // ラベル再表示
            this.kj_form.RefreshFormLabel();  // ラベル再表示
            if(this.kj_form.about_menu_instance == null ||
               this.kj_form.about_menu_instance.IsDisposed) {
                // NOP
            }else{
                // ラベル再表示 (about)
                this.kj_form.about_menu_instance.RefreshFormLabel();  
            }
            
        }

#endif            
        //------------------------------------------------------------------------
        // フォームのラベル文字列を再取得(言語切り替え時の表示切替)
        public void RefreshFormLabel()
        {
            this.exitButton.Text = msg.Get("L006"); 

#if !EDICT
            // 詳細設定
            this.checkBox1.Text   = msg.Get("L015") ;
            this.radio_kana.Text  = msg.Get("L031") ;  
            this.radio_alpha.Text = msg.Get("L032") ;  
            
            this.checkBox2.Text = msg.Get("L016"); // debug mode
            this.checkBox3.Text = msg.Get("L020"); //FreeWnnの単漢字を表示しない
                                                   // (except word of cost:9999)
            this.checkBox4.Text = msg.Get("L023"); //一部の活用形を表示しない
                                                   // (except word of cost:8888)
            this.checkBox6.Text = msg.Get("L033"); // Compact Mode
#endif     
       
            this.checkBox5.Text = msg.Get("L021"); // Clipboard
            this.groupBox1.Text = msg.Get("L029"); // 詳細設定

            // 検索対象
            this.radio_all.Text = msg.Get("L069") ;  
#if !EDICT
            this.radio_kr.Text  = msg.Get("L070") ;  // 韓国語
#else
            this.radio_kr.Text  = msg.Get("L078") ;  // 英語
#endif     
            this.radio_jp.Text  = msg.Get("L071") ;  
            this.groupBox2.Text = msg.Get("L068");  // 検索対象

#if !EDICT
            // 表示モード
            this.radio_culture_en.Text = msg.Get("L078") ;  
            this.radio_culture_kr.Text = msg.Get("L070") ;  
            this.radio_culture_jp.Text = msg.Get("L071") ;  
            this.groupBox3.Text = msg.Get("L077");  // 表示モード

#endif            
            // 初期状態に戻すボタン
            this.resetButton.Text = msg.Get("L030"); 

            // フォント変更，フォントを戻す
            this.fontButton.Text      = msg.Get("L082"); 
            this.resetFontButton.Text = msg.Get("L083"); 
        }

        //------------------------------------------------------------------------
        private void fontdialog_Click(object sender, System.EventArgs e)
        {
            FontDialog fontDialog1 = new FontDialog();

            // 現在のフォントを設定
            fontDialog1.Font  = kj_form.inputArea.Font;
            
            // 選択可のサイズ最大値，最小値を設定
            fontDialog1.MaxSize = 15;
            fontDialog1.MinSize = 7;
            // 存在しないフォントやスタイルをユーザーが選択すると
            // エラーメッセージを表示する
            fontDialog1.FontMustExist = true;
            // 横書きフォントだけを表示する
            fontDialog1.AllowVerticalFonts = false;
            // 取り消し線,下線,色などのオプションを指定不可にする
            fontDialog1.ShowEffects = false;

            //ダイアログを表示する
            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                // フォントを変える
                kj_form.inputArea.Font      = fontDialog1.Font;
                kj_form.richTextBox2.Font   = fontDialog1.Font;
                
                FontConverter fontConverter = new FontConverter();
                kj_form.Setting.font
                    = fontConverter.ConvertToString(fontDialog1.Font);
            }

        }
        //------------------------------------------------------------------------
        private void resetFontdialog_Click(object sender, System.EventArgs e)
        {
            // カスタマイズ情報を消して，
            kj_form.Setting.font = null;

            // フォントを再設定 (デフォルトに戻す)
            kj_form.FontSetting();
            kj_form.inputArea.Font      = kj_form.formFont;
            kj_form.richTextBox2.Font   = kj_form.formFont;
        }

        //------------------------------------------------------------------------
        private void reset_Click(object sender, System.EventArgs e)
        {
            // 保存デフォルト設定に戻す
#if !EDICT
            this.checkBox1.Checked  = false;
            this.radio_kana.Checked = true;
            this.radio_all.Checked  = true;
            this.checkBox2.Checked  = false;
            this.checkBox3.Checked  = false;
            this.checkBox4.Checked  = true;
#endif
            this.checkBox5.Checked  = false;
#if !EDICT
            this.checkBox6.Checked  = false; // Compact Mode
#else
//            this.checkBox6.Checked  = true;  // Compact Mode
//              //  Edictはコンパクトモード固定
#endif
            
            this.kj_form.Setting.withPronunciation = false;
            this.kj_form.Setting.PronunciationType = 1;
            this.kj_form.Setting.TargetLang        = 1;
            this.kj_form.Setting.debugInfo         = false;
            this.kj_form.Setting.except9999        = false;
            this.kj_form.Setting.except8888        = true;
            this.kj_form.Setting.ClipboardView     = false;
#if !EDICT
            this.kj_form.Setting.CompactForm       = false;
#else
            this.kj_form.Setting.CompactForm       = true;
#endif
        }
        

    }



} // end of namespace
