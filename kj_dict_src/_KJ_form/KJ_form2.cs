// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
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
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices; 

[assembly: AssemblyTitle("adhoc Korean-Japnese translator")]

namespace HYAM.KJ_dict
{
    public class KJ_form2 : System.Windows.Forms.Form
    {

        public static StringTrans  kj_stringTrans;  // HYAM.KJ_dict.StringTrans

        public static bool clipboardTranslation;

        // サブフォームを１つしか出さないための記憶域
        private static KJ_form2sub   form2sub_instance;
        private static About_Form    about_menu_instance;

        private static KJ_Message msg;


//        private System.Windows.Forms.Button button1;
//        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox inputArea;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.RichTextBox outputArea;
//        private System.Windows.Forms.TextBox outputArea;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.ContextMenu cm1;        
        private System.Windows.Forms.MenuItem menuCopy1;     
        private System.Windows.Forms.MenuItem menuPaste1;    
        private System.Windows.Forms.ContextMenu cm2;        
        private System.Windows.Forms.MenuItem menuCopy2;     
        private System.Windows.Forms.MenuItem menuPaste2;    
        private System.Windows.Forms.ContextMenu cm3;        
        private System.Windows.Forms.MenuItem menuPreference;     
        private System.Windows.Forms.MenuItem menu_about;     

        private System.Windows.Forms.ProgressBar  pBar1;

        private System.ComponentModel.Container components = null;

        // for Clipboard
        private IntPtr NextHandle; 
        [DllImport("user32")] 
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer); 
        [DllImport("user32", CharSet=CharSet.Auto)] 
        public extern static int SendMessage(IntPtr hWnd, int Msg, 
                                             IntPtr wParam, 
                                             IntPtr lParam); 
        private const int WM_DRAWCLIPBOARD = 0x0308; 
        private const int WM_CHANGECBCHAIN = 0x030D; 


        // コンストラクタ
        public KJ_form2()
        {
            msg = new KJ_Message();
            InitializeComponent();
    
            KJ_dict.inputIsHangul = true;  // default
            KJ_form2.clipboardTranslation = false;
            
            //  StringTransのコンストラクタ
            kj_stringTrans = new StringTrans(this.pBar1, true); // 2009.05.27
         //   kj_stringTrans = new StringTrans(this.pBar1);
            
            // kj_stringTrans.HtmlTagSkip = true;  
            
            // for Clipboard
            NextHandle = SetClipboardViewer(this.Handle); 
        }
        

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

        private void InitializeComponent()
        {
//            this.button1 = new System.Windows.Forms.Button();
            this.inputArea = new System.Windows.Forms.TextBox();
            this.outputArea = new System.Windows.Forms.RichTextBox();
//            this.outputArea = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();

            this.pBar1 = new System.Windows.Forms.ProgressBar();

            this.panel1.SuspendLayout();
            this.SuspendLayout();
            
            //================================================================ 
            this.menuCopy1  = new System.Windows.Forms.MenuItem();
            this.menuPaste1 = new System.Windows.Forms.MenuItem();
            // menuCopy
            this.menuCopy1.Index = 1;
            this.menuCopy1.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuCopy1.Text = "Copy (&C)";
            this.menuCopy1.Click += new System.EventHandler(this.menuCopy_Click1);
            // menuPaste
            this.menuPaste1.Index = 2;
            this.menuPaste1.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuPaste1.Text = "Paste (&P)";
            this.menuPaste1.Click += new System.EventHandler(this.menuPaste_Click1);
            MenuItem[] menuItem1;
            menuItem1 = new MenuItem[] {
                this.menuCopy1 ,
                this.menuPaste1 
            };
            this.cm1 = new ContextMenu(menuItem1);
            
            
            //-------------------------------------------------------
            this.menuCopy2  = new System.Windows.Forms.MenuItem();
            this.menuPaste2 = new System.Windows.Forms.MenuItem();
            // menuCopy
            this.menuCopy2.Index = 1;
            this.menuCopy2.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuCopy2.Text = "Copy (&C)";
            this.menuCopy2.Click += new System.EventHandler(this.menuCopy_Click2);
            // menuPaste
            this.menuPaste2.Index = 2;
            this.menuPaste2.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuPaste2.Text = "Paste (&P)";
            this.menuPaste2.Click += new System.EventHandler(this.menuPaste_Click2);
            MenuItem[] menuItem2;
            menuItem2 = new MenuItem[] {
                this.menuCopy2 ,
                this.menuPaste2 
            };
            this.cm2 = new ContextMenu(menuItem2);
            
            
            //-------------------------------------------------------
            this.menuPreference  = new System.Windows.Forms.MenuItem();
            this.menu_about      = new System.Windows.Forms.MenuItem(); 

            // menuDebug
            this.menuPreference.Index = 1;
            this.menuPreference.Text = msg.Get("L007") ;
            this.menuPreference.Click += new System.EventHandler
                                                (this.cm3_Click);
            // menu_about
            this.menu_about.Text = "about ..." ;   
            this.menu_about.Click += new System.EventHandler
                                                (this.menu_about_menu_Click);

            MenuItem[] menuItem3;
            menuItem3 = new MenuItem[] {
                this.menuPreference ,
                this.menu_about 
            };
            this.cm3 = new ContextMenu(menuItem3);


            //================================================================ 
            // 
            // button1
            // 
//            this.button1.Location = new System.Drawing.Point(0, 0);
//            this.button1.Name = "button1";
//            this.button1.Size = new System.Drawing.Size(70, 20);
//            this.button1.TabIndex = 0;
//            this.button1.Text = msg.Get("L008");  // translate
//            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(520, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = msg.Get("L005");  // exit
            this.button2.Click += new System.EventHandler(this.button2_Click);

            //------------------------------------------------------------
            // 
            // Main panel 
            // 
            this.panel1.ContextMenu  = this.cm3;  // hidden preference :-P
//            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 20);
            this.panel1.TabIndex = 12;
            
            // 
            // inputArea
            // 
            this.inputArea.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputArea.Location = new System.Drawing.Point(8, 45);
            this.inputArea.Name = "inputArea";
            this.inputArea.Size = new System.Drawing.Size(550, 180);
            this.inputArea.TabIndex = 1;
            this.inputArea.Text = "";
            this.inputArea.Multiline = true;
            this.inputArea.ContextMenu = this.cm1;
            //入力エリアが変化したら自動処理          
            this.inputArea.TextChanged  += new System.EventHandler(this.Text_Changed);

            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(8, 235);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(550, 3);
            this.splitter1.TabIndex = 14;
            this.splitter1.TabStop = false;

            // 
            // outputArea
            // 
            this.outputArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputArea.Location = new System.Drawing.Point(8, 230);
            this.outputArea.Name = "outputArea";
            this.outputArea.Size = new System.Drawing.Size(550, 185);
            this.outputArea.TabIndex = 2;
            this.outputArea.Text = "";
            this.outputArea.Multiline = true;
            this.outputArea.ContextMenu = this.cm2;

            // 
            // pBar1
            // 
            this.pBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pBar1.Location = new System.Drawing.Point(14, 430);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(540, 10);
            this.pBar1.TabIndex = 11;

            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(570, 445);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.outputArea);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.inputArea);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "KJ_form2 ( Debug tool )";
            this.ResumeLayout(false);

        }

        [STAThread]
        static void Main() 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new KJ_form2());
        }
        
        

        //--------------------------------------------------------------------------
        // クリップボードの監視
        protected override void WndProc(ref Message msg) { 
            switch (msg.Msg) { 
            case WM_DRAWCLIPBOARD: 
                // クリップボード翻訳onの時
                if(KJ_form2.clipboardTranslation ){
                    this.inputArea.Text = 
                        (string)Clipboard.GetDataObject().GetData(DataFormats.UnicodeText); 
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

        // inputAreaの文字が変化したら翻訳。
        private void Text_Changed(object sender, System.EventArgs e)
        {
            form_search();
        }

        // 翻訳処理
        private void form_search()
        {  
            this.pBar1.Visible = true;
            this.pBar1.Minimum = 1;
            this.pBar1.Value   = 1;
            this.pBar1.Step    = 1;

            this.outputArea.Text = kj_stringTrans.Trans(this.inputArea.Text) ;
        }

        //---------------------------------------------------------------------
//        private void button1_Click(object sender, System.EventArgs e)
//        { 
//            form_search();
//        }
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        
        // Panel上のコンテキストメニュー
        private void cm3_Click(object sender, System.EventArgs e)
        {
           
           // モードレス
           if(form2sub_instance == null || form2sub_instance.IsDisposed) {
               form2sub_instance = new KJ_form2sub();
               form2sub_instance.Show();
           }else{
                if( form2sub_instance != null){
                    form2sub_instance.Focus() ;
                }
           }

           // モーダル
           // KJ_form2sub f2sub = new KJ_form2sub();
           // f2sub.ShowDialog(this);
           // f2sub.Dispose();

        }

        //---------------------------------------------------------------------
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
        //-----------------------------------------------------------------------------
        private void menuCopy_Click2(object sender, System.EventArgs e)
        {
            if(this.outputArea.SelectionLength > 0) {
                this.outputArea.Copy();       
            }
        }
        private void menuPaste_Click2(object sender, System.EventArgs e)
        {
            if(Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                this.outputArea.Paste();      
           }
        }
        
        //---------------------------------------------------------------------
        // about ...
        private void menu_about_menu_Click(object sender, System.EventArgs e)
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

    } // end of KJ_form2


//===========================================================================
    public class KJ_form2sub : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.ComponentModel.Container components = null;

        private static KJ_Message msg;

        public KJ_form2sub()
        {
            msg = new KJ_Message();
            InitializeComponent();
            
            
            if(KJ_form2.kj_stringTrans.AutoConv){
                this.checkBox1.Checked = true;
            }else{
                this.checkBox1.Checked = false;
            }
            
            if(KJ_form2.kj_stringTrans.DebugInfo){
                this.checkBox2.Checked = true;
            }else{
                this.checkBox2.Checked = false;
            }
            
            if(!KJ_dict.inputIsHangul){
                this.checkBox3.Checked = true; // J-->K direction
            }else{
                this.checkBox3.Checked = false;
            }
        }


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
        
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();

            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            
            this.groupBox1.SuspendLayout();

            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(230, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 20);
            this.button1.TabIndex = 3;
            this.button1.Text =  msg.Get("L006");
            this.button1.Click += new System.EventHandler(this.button1_Click);
            
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(25, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(250, 20);
            this.checkBox1.Text = msg.Get("L017");
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_Check);

            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(25, 50);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(200, 20);
            this.checkBox2.Text = msg.Get("L016");
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_Check);

            // 
            // checkBox3
            // 
            this.checkBox3.Location = new System.Drawing.Point(25, 80);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(250, 20);
            this.checkBox3.Text = msg.Get("L018");
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_Check);

            // 
            // checkBox4   
            // 
            this.checkBox4.Location = new System.Drawing.Point(25, 110);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(250, 20);
            this.checkBox4.Text = msg.Get("L021");  // Clipboard
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_Check);
            
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(
                             new System.Windows.Forms.Control[] {
                                                 this.checkBox1 ,
                                                 this.checkBox2 ,
                                                 this.checkBox3 ,
                                                 this.checkBox4  
                                            });
            this.groupBox1.Location = new System.Drawing.Point(10, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 150);
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = msg.Get("L029");

            // 
            // Form2sub
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(300, 200);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form2sub";
            this.Text = "KJ_form2 Extended Menu";
            this.ResumeLayout(false);


        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        
        
        private void checkBox1_Check(object sender, System.EventArgs e)
        {   
            if(this.checkBox1.Checked ){
                KJ_form2.kj_stringTrans.AutoConv = true;
            }else{
                KJ_form2.kj_stringTrans.AutoConv = false;
            }
        }
        private void checkBox2_Check(object sender, System.EventArgs e)
        {   
            if(this.checkBox2.Checked ){
                KJ_form2.kj_stringTrans.DebugInfo = true;
            }else{
                KJ_form2.kj_stringTrans.DebugInfo = false;
            }
        }
        private void checkBox3_Check(object sender, System.EventArgs e)
        {   
            if(this.checkBox3.Checked ){
                KJ_dict.inputIsHangul = false; // J-->K
            }else{
                KJ_dict.inputIsHangul = true; 
            }
        }
        private void checkBox4_Check(object sender, System.EventArgs e)
        {   
            if(this.checkBox4.Checked ){
                KJ_form2.clipboardTranslation = true;
            }else{
                KJ_form2.clipboardTranslation = false;
            }
        }

    } // end of KJ_form2sub
}
