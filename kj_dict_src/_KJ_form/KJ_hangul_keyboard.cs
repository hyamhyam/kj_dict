// Copyright 2007, 2009 hyam <hyamhyam@gmail.com>
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
    
#if !EDICT

public class Hangul_keyboard_Form : System.Windows.Forms.Form
{
    public static string []  keyTopCharAlpha = { 
        "Q",  "W",   "E",  "R",  "T",  "Y",  "U",  "I",  "O",  "P",
        "A",  "S",   "D",  "F",  "G",  "H",  "J",  "K",  "L",
        "Z",  "X",   "C",  "V",  "B",  "N",  "M",  "",   "",   ""
    };
    public static string []  keyTopCharAlphaLower = { 
        "q",  "w",  "e",  "r",  "t", "y",   "u",  "i",  "o",  "p",
        "a",  "s",  "d",  "f",  "g",  "h",  "j",  "k",  "l",
        "z",  "x",  "c",  "v",  "b",  "n",  "m",  "",   "",   ""
    };

    public static string []  keyTopCharHangul = { 
        "ㅂ",  "ㅈ",   "ㄷ",  "ㄱ",  "ㅅ",  "ㅛ",  "ㅕ",  "ㅑ",  "ㅐ", "ㅔ",
        "ㅁ",  "ㄴ",   "ㅇ",  "ㄹ",  "ㅎ",  "ㅗ",  "ㅓ",  "ㅏ",  "ㅣ",
        "ㅋ",  "ㅌ",   "ㅊ",  "ㅍ",  "ㅠ",  "ㅜ",  "ㅡ",  "",    "",   ""
    };


    public static bool keyshiftdown=false;
    
    private CharButton [] ButtonList  ;
    
    private CharButton clearButton;
//    private CharButton exitButton;
    
    
    public KJ_form kj_form; 
    
    public Hangul_keyboard_Form(KJ_form parent)
    {
        this.kj_form = parent; // 表示するKeyboardに現在のKJ_formを設定
        
        InitializeComponent();
        
    }
    
    private void InitializeComponent()
    {
        
        const int ButtonNumMax  = 26;
        
        const int ButtonHeight = 40;
        const int ButtonWidth  = 50;
        const int startColumn  = 10;

        const int leftMargin   = 10;
        const int topMargin    = 20;
        
        const int mainFormHeight = 170;
        const int mainFormWidth  = 550;
        
        int row    = 0;
        int column = startColumn;
        
        this.SuspendLayout();
         
        ButtonList = new CharButton[ButtonNumMax] ;
        
        // キーボード 3段
        for(int i = 0 ; i < ButtonNumMax; i++){
            if(i == 10 || i == 19 ){ 
                row += ButtonHeight; 
                if(i==10){
                    column = startColumn + (ButtonWidth/2); 
                }else{
                    column = startColumn + ButtonWidth; 
                }
            }
            
            ButtonList[i] = new CharButton();
            
            ButtonList[i].Key  = keyTopCharAlphaLower[i] ;
            ButtonList[i].Text = keyTopCharAlpha[i] + " " + keyTopCharHangul[i] ;

            ButtonList[i].Size     = new System.Drawing.Size(ButtonWidth, 
                                                             ButtonHeight);
            ButtonList[i].Location = new System.Drawing.Point(leftMargin + column, 
                                                              topMargin  + row );
            
            column += ButtonWidth;

            ButtonList[i].Click += new System.EventHandler(this.Button_Click);
            this.Controls.Add( ButtonList[i] ); 
        }

        this.clearButton = new CharButton();
        this.clearButton.Text = "clear"; 
        this.clearButton.Size     = new System.Drawing.Size(ButtonWidth, 
                                                           (ButtonHeight/2) );
        this.clearButton.Location = new System.Drawing.Point( 
                                         leftMargin + column + (ButtonWidth/2), 
                                    //     leftMargin + column + ButtonWidth, 
                                         topMargin  + row + (ButtonHeight/2) );
        this.clearButton.Click += new System.EventHandler(this.clear_Click);
        this.Controls.Add(this.clearButton); 
        column += (ButtonWidth/2);

        
//        this.exitButton = new CharButton();
//        this.exitButton.Text = "close"; 
//        this.exitButton.Size     = new System.Drawing.Size(ButtonWidth, 
//                                                           (ButtonHeight/2) );
//        this.exitButton.Location = new System.Drawing.Point( 
//                                         leftMargin + column + ButtonWidth, 
//                                         topMargin  + row + (ButtonHeight/2) );
//        this.exitButton.Click += new System.EventHandler(this.exit_Click);
//        this.Controls.Add(this.exitButton); 


        this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
        this.ClientSize = new System.Drawing.Size(mainFormWidth, 
                                                  mainFormHeight);
        this.Name = "MainForm";
        this.Text = "Hangul Keyboard";

        //キーイベントをフォームで受け取る
        this.KeyPreview = true; 
        this.KeyPress += new KeyPressEventHandler(this.Form_KeyPress);
        this.KeyUp    += new KeyEventHandler(this.Form_KeyUp);
        this.KeyDown  += new KeyEventHandler(this.Form_KeyDown);

        // ウィンドの閉じる[X]が押されたときのイベント登録(closeと同じ)
        this.FormClosing += new FormClosingEventHandler(this.exit_Click);

        // フォームの位置を調整可能にする
        this.StartPosition = FormStartPosition.Manual;
        // 位置設定
        this.Left = this.kj_form.Left;
        this.Top  = this.kj_form.Top + this.kj_form.Height ;

        this.ResumeLayout(false);

    } // end of InitializeComponent
    
    //------------------------------------------------------------------------
    // 押された特殊キーを拾う
    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        // エスケープキーが押されたら入力エリアをクリア
        if (e.KeyCode == Keys.Escape){
            // ハングルキーボードが出ているが本体側にフォーカスがある場合
            this.kj_form.inputKey = "";
            this.kj_form.inputArea.Text = "";
            return;
        }

        // backSpaceが押され，1文字以上残っている場合
        if (e.KeyCode == Keys.Back && this.kj_form.inputKey.Length > 0)
        {
            this.kj_form.inputKey = 
                  this.kj_form.inputKey.Remove(this.kj_form.inputKey.Length - 1);

            if(this.kj_form != null){
                this.kj_form.inputArea.Text =  
                      HYAM.Lingua.Hangul.KeyString2Hangul(this.kj_form.inputKey); 
            }
        }
        
        // Shiftが押された場合
        if (e.Shift)
        {
           // MessageBox.Show("Shift Down", "warning",  
           //                 MessageBoxButtons.OK , MessageBoxIcon.Exclamation );

            ButtonList[0].Text = keyTopCharAlpha[0] + " " + "ㅃ";
            ButtonList[1].Text = keyTopCharAlpha[1] + " " + "ㅉ";
            ButtonList[2].Text = keyTopCharAlpha[2] + " " + "ㄸ";
            ButtonList[3].Text = keyTopCharAlpha[3] + " " + "ㄲ";
            ButtonList[4].Text = keyTopCharAlpha[4] + " " + "ㅆ";
            ButtonList[8].Text = keyTopCharAlpha[8] + " " + "ㅒ";
            ButtonList[9].Text = keyTopCharAlpha[9] + " " + "ㅖ";
            
            keyshiftdown=true;
        }else{
            keyshiftdown=false;
        }
      
    }
    //------------------------------------------------------------------------
    // 特殊キーが離されたイベントを拾う
    private void Form_KeyUp(object sender, KeyEventArgs e)
    {
        // Shiftが押されて，離された場合
        if (keyshiftdown && !e.Shift)
        {
            //MessageBox.Show("Shift Up", "warning",  
            //                MessageBoxButtons.OK , MessageBoxIcon.Exclamation );
                            
            ButtonList[0].Text = keyTopCharAlpha[0] + " " + keyTopCharHangul[0] ;
            ButtonList[1].Text = keyTopCharAlpha[1] + " " + keyTopCharHangul[1];
            ButtonList[2].Text = keyTopCharAlpha[2] + " " + keyTopCharHangul[2];
            ButtonList[3].Text = keyTopCharAlpha[3] + " " + keyTopCharHangul[3];
            ButtonList[4].Text = keyTopCharAlpha[4] + " " + keyTopCharHangul[4];
            ButtonList[8].Text = keyTopCharAlpha[8] + " " + keyTopCharHangul[8];
            ButtonList[9].Text = keyTopCharAlpha[9] + " " + keyTopCharHangul[9];
            keyshiftdown=false;
        }
    }    
    //------------------------------------------------------------------------
    // 押されたキーを拾う
    private void Form_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (CodeCheck.IsAlpah(e.KeyChar))
        {
        //  this.kj_form.inputKey = 
        //        HYAM.Lingua.Hangul.Hangul2KeyString(this.kj_form.inputArea.Text);
            this.kj_form.inputKey += e.KeyChar;
        }else{
            return;
        }

        this.kj_form.inputArea.Text = 
              HYAM.Lingua.Hangul.KeyString2Hangul(this.kj_form.inputKey); ;
    }

    private void Button_Click(object sender, System.EventArgs e)
    {
        //this.kj_form.inputKey = 
        //      HYAM.Lingua.Hangul.Hangul2KeyString(this.kj_form.inputArea.Text);
        
        string clickKey = ((CharButton)sender).Key;
        if(keyshiftdown == true){
            if(clickKey == "q"){ clickKey = "Q"; }
            else if(clickKey == "w"){ clickKey = "W"; }
            else if(clickKey == "e"){ clickKey = "E"; }
            else if(clickKey == "r"){ clickKey = "R"; }
            else if(clickKey == "t"){ clickKey = "T"; }
            else if(clickKey == "o"){ clickKey = "O"; }
            else if(clickKey == "p"){ clickKey = "P"; }
        }
        this.kj_form.inputKey += clickKey;
        this.kj_form.inputArea.Text = 
              HYAM.Lingua.Hangul.KeyString2Hangul(this.kj_form.inputKey);
    }
    //------------------------------------------------------------------------
    private void clear_Click(object sender, System.EventArgs e)
    {
        this.kj_form.inputKey = "";
        this.kj_form.inputArea.Text = "";
    }
    //------------------------------------------------------------------------
    private void exit_Click(object sender, System.EventArgs e)
    {
        // 入力抑止を解除
        this.kj_form.inputArea.ReadOnly = false; //[2009/08/16 23:55:20]
        
       // this.kj_form.inputArea.Text = "";
        this.kj_form.hangul_keyboard_instance = null;
        
        // this.Close();
    }

//-----------------------------------------------------------------
class CharButton :  System.Windows.Forms.Button  {
    public string Key;
    
    public CharButton(){
        this.Key="";
        
        // フォーカスがこないようにする
        this.SetStyle(ControlStyles.Selectable, false);
        
    }
}
    

}

#endif  // end of class



} // end of namespace
