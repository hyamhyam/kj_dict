// Copyright 2004, 2012 hyam <hyamhyam@gmail.com>
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


namespace HYAM.KJ_dict
{

    public class About_Form : System.Windows.Forms.Form
    {

        private System.Windows.Forms.Button exitButton;


        private System.Windows.Forms.Label progname;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Label help_title;
        private System.Windows.Forms.Label latest_title;
        private System.Windows.Forms.Label author;
        
        private System.Windows.Forms.LinkLabel LinkLabel1;
        private System.Windows.Forms.LinkLabel LinkLabel2;
        
        
        private static KJ_Message msg;


        // コンストラクタ
        public About_Form()
        {
            msg = new KJ_Message();
            InitializeComponent();

        }
        
        private void InitializeComponent()
        {
            const string AssemblyVersion = "4.0.3.0" ;
            
            const int Buttonheight  = 20;

            const int leftMargin   = 10;
            const int topMargin    = 20;
            const int labelMargin  = 20;
            
            const int mainFormWidth    = 340;
            const int mainFormHight    = 240;
            
            int labelposX;
            int label_width = mainFormWidth - leftMargin*2 ;

            // "close" 
            this.exitButton = new System.Windows.Forms.Button();
            this.exitButton.Size     = new System.Drawing.Size(60, Buttonheight);
            this.exitButton.Location = 
                  new System.Drawing.Point( mainFormWidth - 70, 
                                            mainFormHight - Buttonheight - 10 );
            this.exitButton.Click += new System.EventHandler(this.exit_Click);
            this.Controls.Add(this.exitButton); 


            // main title
            labelposX = topMargin;
            this.progname = new System.Windows.Forms.Label();
            this.progname.Location = 
                 new System.Drawing.Point(leftMargin  , labelposX  );
            this.progname.Size = new System.Drawing.Size(label_width, Buttonheight);
         //   this.progname.Name = "label1";

#if !EDICT
            this.progname.Text = "Very simple Korean-Japanese dictionary KJ_dict";
#else
            this.progname.Text = "Very simple English-Japanese dictionary KJ_Edict";
#endif

            this.Controls.Add(this.progname); 

            // version 
            labelposX += labelMargin;
            this.version = new System.Windows.Forms.Label();
            this.version.Location = 
                new System.Drawing.Point(leftMargin + 30 , labelposX  );
            this.version.Size = new System.Drawing.Size(label_width, Buttonheight);
            this.version.Text = "version " + AssemblyVersion ; 
            this.Controls.Add(this.version); 

            // author 
            labelposX += labelMargin;
            this.author = new System.Windows.Forms.Label();
            this.author.Location = 
                 new System.Drawing.Point(leftMargin + 30 , labelposX  );
            this.author.Size = new System.Drawing.Size(label_width, Buttonheight);
            this.author.Text = "author : hyamhyam@gmail.com" ; 
            this.Controls.Add(this.author); 
            
            // help_title 
            labelposX += labelMargin;
            labelposX += labelMargin;
            this.help_title = new System.Windows.Forms.Label();
            this.help_title.Location =
                new System.Drawing.Point(leftMargin  , labelposX  );
            this.help_title.Size = 
                new System.Drawing.Size(label_width, Buttonheight);
            this.Controls.Add(this.help_title); 
            
            // マニュアルURLのリンク
            labelposX += labelMargin;
            this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.LinkLabel1.Location = 
                 new System.Drawing.Point(leftMargin , labelposX );
            this.LinkLabel1.Size = 
                 new System.Drawing.Size(label_width, Buttonheight);
            this.LinkLabel1.Name = "LinkLabel1";
            this.LinkLabel1.Click += new System.EventHandler(this.LinkLabel1_Click);
            this.LinkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 100);
            this.Controls.Add(this.LinkLabel1); 

            // title 最新版の確認
            labelposX += labelMargin;
            labelposX += labelMargin;
            this.latest_title = new System.Windows.Forms.Label();
            this.latest_title.Location = 
                new System.Drawing.Point(leftMargin  , labelposX  );
            this.latest_title.Size = 
                new System.Drawing.Size(label_width, Buttonheight);
            this.Controls.Add(this.latest_title); 
            
            // 最新版URLのリンク
            labelposX += labelMargin;
            this.LinkLabel2 = new System.Windows.Forms.LinkLabel();
            this.LinkLabel2.Location = 
                new System.Drawing.Point(leftMargin , labelposX );
            this.LinkLabel2.Size = 
                new System.Drawing.Size(label_width, Buttonheight);
            this.LinkLabel2.Name = "LinkLabel2";
            this.LinkLabel2.Click += new System.EventHandler(this.LinkLabel2_Click);
            this.LinkLabel2.LinkArea = new System.Windows.Forms.LinkArea(0, 100);
            this.Controls.Add(this.LinkLabel2); 

            this.ClientSize = new System.Drawing.Size(mainFormWidth, mainFormHight);
            this.Name = "KJ_form_about_menu";
            
            this.RefreshFormLabel();          // ラベル再表示

        }  // end of InitializeComponent
        
        //------------------------------------------------------------------------
        public void RefreshFormLabel()
        {
            this.exitButton.Text = msg.Get("L006");  // 閉じる

            this.help_title.Text = msg.Get("L024") ;   // 簡単なマニュアル
          
            this.latest_title.Text = msg.Get("L026");  // 最新版の確認
#if !EDICT
            this.Text = msg.Get("L081") ; // about;
            
            this.LinkLabel1.Text = msg.Get("L027")  ;   // manual URL
            
//            this.LinkLabel2.Text   = msg.Get("L028") ;
            this.LinkLabel2.Text = "http://code.google.com/p/kjdict/downloads/list" ;
#else
            this.Text = msg.Get("L084") ; // about;
            this.LinkLabel1.Text = msg.Get("L027")  ;   // manual URL;   
            
            this.LinkLabel2.Text = "http://code.google.com/p/kjedict/downloads/list" ;
#endif
        }
        //------------------------------------------------------------------------
        private void exit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        
        //------------------------------------------------------------------------
        private void LinkLabel1_Click(object sender, System.EventArgs e)
        {
            //リンク先に移動したことにする
            LinkLabel1.LinkVisited = true;
            //ブラウザで開く
            System.Diagnostics.Process.Start(this.LinkLabel1.Text);
        }
        //------------------------------------------------------------------------
        private void LinkLabel2_Click(object sender, System.EventArgs e)
        {
            //リンク先に移動したことにする
            LinkLabel2.LinkVisited = true;

            //ブラウザで開く
            System.Diagnostics.Process.Start(this.LinkLabel2.Text);

        }
        

    }



} // end of namespace
