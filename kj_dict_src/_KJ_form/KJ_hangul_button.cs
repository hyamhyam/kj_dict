// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
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

public class Hangul_Button_Form : System.Windows.Forms.Form
{

    private CharButton [] ConsonantButtonList  ;
    private CharButton [] VowelButtonList  ;
    private CharButton [] PachimButtonList  ;
    
    private System.Windows.Forms.Button exitButton;
    private System.Windows.Forms.Button backspaceButton;
    private System.Windows.Forms.Button clearButton;
    
    private System.Windows.Forms.RichTextBox richTextBox1;
    private System.Windows.Forms.RichTextBox richTextBox2;
    
    // OSチェック
    private bool is2000orXP = true; // Windows98系はもうサポートしない

    // キーの記憶域
    private ArrayList parts_data; // V1.5.0.0～
    
    
    public KJ_form kj_form; 
    
    public Hangul_Button_Form(KJ_form parent)
    {
        this.kj_form = parent;
        //  this.is2000orXP = KJ_Util.Is2000orXP();
        InitializeComponent();
        
        this.kj_form.inputArea.Enabled = false;
    }
    
    private void InitializeComponent()
    {
        this.parts_data = new ArrayList() ;
        
        const int ConsonantButtonNumMax  = 19;
        const int VowelButtonNumMax      = 21;
        const int PachimButtonNumMax     = 27;
        const int ButtonSize = 30;
        const int startColumn  = 10;
        const int startRow     = 10;

        const int leftMargin   = 10;
        const int topMargin    = 20;
        
        const int mainFormHeight = 440;
        const int mainFormWidth  = 380;
        
        int row    = startRow;
        int column = startColumn;

        this.exitButton = new System.Windows.Forms.Button();
        this.exitButton.Text = "close"; 
        this.exitButton.Size     = new System.Drawing.Size(40, 20);
        this.exitButton.Location = 
            new System.Drawing.Point( mainFormWidth - 50, topMargin );
        this.exitButton.Click += new System.EventHandler(this.exit_Click);
        this.Controls.Add(this.exitButton); 
        
        this.backspaceButton = new System.Windows.Forms.Button();
        this.backspaceButton.Text = "BS"; 
        this.backspaceButton.Size     = new System.Drawing.Size(40, 20);
        this.backspaceButton.Location = new System.Drawing.Point( mainFormWidth - 50 , 
                                                                  topMargin + 50 );
        this.backspaceButton.Click += new System.EventHandler(this.bs_Click);
        this.Controls.Add(this.backspaceButton); 

        this.clearButton = new System.Windows.Forms.Button();
        this.clearButton.Text = "clear"; 
        this.clearButton.Size     = new System.Drawing.Size(40, 20);
        this.clearButton.Location = new System.Drawing.Point( mainFormWidth - 50 , 
                                                              topMargin + 100 );
        this.clearButton.Click += new System.EventHandler(this.clear_Click);
        this.Controls.Add(this.clearButton); 
        
        this.richTextBox1 = new System.Windows.Forms.RichTextBox();
        this.richTextBox1.Location = 
            new System.Drawing.Point(leftMargin + startColumn, 
                                     mainFormHeight - 75);
        this.richTextBox1.Size =
           new System.Drawing.Size(mainFormWidth - 50, 25);
        this.richTextBox1.ReadOnly = true ;
        this.richTextBox1.BackColor  = Color.Silver ;
        this.Controls.Add(this.richTextBox1); 
        
        this.richTextBox2 = new System.Windows.Forms.RichTextBox();
        this.richTextBox2.Location = 
             new System.Drawing.Point(leftMargin + startColumn,
                                      mainFormHeight - 35);
        this.richTextBox2.Size =
             new System.Drawing.Size(mainFormWidth - 50 , 25);
        this.richTextBox2.ReadOnly = true ;
        this.richTextBox2.BackColor  = Color.Silver ;
        this.Controls.Add(this.richTextBox2); 
         
        ConsonantButtonList = new CharButton[ConsonantButtonNumMax] ;
        VowelButtonList     = new CharButton[VowelButtonNumMax] ;
        PachimButtonList    = new CharButton[PachimButtonNumMax] ;
        
        // 子音
        for(int i = 0 ; i < ConsonantButtonNumMax; i++){
            ConsonantButtonList[i] = new CharButton();
            
            ConsonantButtonList[i].Parts  = HYAM.Lingua.Hangul.ConsonantPartsList[i] ;

            if(this.is2000orXP){
                ConsonantButtonList[i].Text = HYAM.Lingua.Hangul.ConsonantJamoList[i] ;
            }else{
                // for Windows98
                ConsonantButtonList[i].Text = HYAM.Lingua.Hangul.ConsonantList[i] ;
            }

            ConsonantButtonList[i].Size     = new System.Drawing.Size(ButtonSize, ButtonSize);
            ConsonantButtonList[i].Location = new System.Drawing.Point( leftMargin+column, 
                                                                        topMargin + row );
            column += ButtonSize;
            if(i%10==9){
                row   += ButtonSize;
                column = startColumn;
            }
            ConsonantButtonList[i].Click += new System.EventHandler(this.Button_Click);
            this.Controls.Add( ConsonantButtonList[i] ); 
        }
        
        // 母音 ---------------------------------------------------------------
        column = startColumn;
        row   += (ButtonSize *2);
        for(int i = 0 ; i < VowelButtonNumMax; i++){
            VowelButtonList[i] = new CharButton();
            
            VowelButtonList[i].Parts  = HYAM.Lingua.Hangul.VowelPartsList[i] ;
            if(this.is2000orXP){
                VowelButtonList[i].Text = HYAM.Lingua.Hangul.VowelJamoList[i] ;
            }else{
                VowelButtonList[i].Text = HYAM.Lingua.Hangul.VowelList[i] ;
            }
            
            VowelButtonList[i].Size = 
                 new System.Drawing.Size(ButtonSize, ButtonSize);
            VowelButtonList[i].Location =
                 new System.Drawing.Point( leftMargin+column, 
                                           topMargin + row );
            column += ButtonSize;
            if(i%10==9){
                row   += ButtonSize;
                column = startColumn;
            }
            VowelButtonList[i].Click += new System.EventHandler(this.Button_Click);
            this.Controls.Add( VowelButtonList[i] ); 
        }
        
        // パッチム -----------------------------------------------------------
        column = startColumn;
        row   += (ButtonSize *2);
        for(int i = 0 ; i < PachimButtonNumMax; i++){
            PachimButtonList[i] = new CharButton();
            
            PachimButtonList[i].Parts  = HYAM.Lingua.Hangul.PachimPartsList[i+1] ;
            if(this.is2000orXP){
                PachimButtonList[i].Text = HYAM.Lingua.Hangul.PachimJamoList[i+1] ;
            }else{
                PachimButtonList[i].Text = HYAM.Lingua.Hangul.PachimList[i+1] ;
            }

            PachimButtonList[i].Size =
                 new System.Drawing.Size(ButtonSize, ButtonSize);
            PachimButtonList[i].Location = 
                new System.Drawing.Point( leftMargin+column, 
                                          topMargin + row );
            column += ButtonSize;
            if(i%10==9){
                row   += ButtonSize;
                column = startColumn;
            }
            PachimButtonList[i].Click += new System.EventHandler(this.Button_Click);
            
            this.Controls.Add( PachimButtonList[i] ); 
        }
        
        
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
        this.ClientSize = 
            new System.Drawing.Size(mainFormWidth, mainFormHeight);
        this.Name = "MainForm";
        this.Text = "Hangul Input method";

        // ウィンドの閉じる[X]が押されたときのイベント登録
        this.FormClosing += 
           new FormClosingEventHandler(this.KJ_hangulbuttonform_FormClosing);
        
        // フォームの位置を調整可能にする
        this.StartPosition = FormStartPosition.Manual;
        // 位置設定
        this.Left = this.kj_form.Right;
        this.Top  = this.kj_form.Top  ;
        
    }
    

    //-------------------------------------------------------------------------
    private void Button_Click(object sender, System.EventArgs e)
    {
        this.parts_data.Add(((CharButton)sender).Parts);
        refreshString() ;
    }
    //-------------------------------------------------------------------------
    
    // Formの閉じるボタン（右上の[X]）が押された場合。
    private void KJ_hangulbuttonform_FormClosing(object sender, 
                                                 FormClosingEventArgs e)
    {
        // 通常検索に戻す
        KJ_dict.DictOpen("KJ_dict.yml");

        this.kj_form.inputArea.Enabled = true;   // 入力可能に戻す
    }
    
    private void exit_Click(object sender, System.EventArgs e)
    {
        // 通常検索に戻す
        KJ_dict.DictOpen("KJ_dict.yml");
        
        this.kj_form.inputArea.Enabled = true;  // 入力可能に戻す

        this.Close();
    }
    
    private void clear_Click(object sender, System.EventArgs e)
    {
        this.kj_form.inputArea.Text = "";
        this.kj_form.inputKey = "" ;  // keyboardとbutton同時使用時のクリア

        this.parts_data = new ArrayList() ; // 初期化

        this.richTextBox1.Text = "";
        this.richTextBox2.Text = "";
    }
    
    private void bs_Click(object sender, System.EventArgs e)
    { 
        if(this.parts_data.Count <= 0 ) {
            return;
        }

        // キースタックから１キー削除し、再表示・再検索
        this.parts_data.RemoveAt( this.parts_data.Count - 1);
        
        refreshString() ;
    }
    
    //------------------------------------------------------------------------
    // 文字ボタンが押されたり，BSキーが押されたときに，
    // 検索文字列を変更し，再検索する。
    private void refreshString()
    {   

        this.richTextBox1.Text =  
                HYAM.Lingua.Hangul.PartsArray2HangulString(this.parts_data) ;
        
        string parts="";
        foreach (string pdata in this.parts_data) {
            parts += pdata;
        }
        this.richTextBox2.Text = parts;
        
        if(this.kj_form != null){
            this.kj_form.inputArea.Text  =  this.richTextBox2.Text ;

            this.kj_form.form_search(  );
        }

    }

}


//-----------------------------------------------------------------
class CharButton :  System.Windows.Forms.Button  {
  //  public string Key;
    public string Parts;
    
    public CharButton(){
    //    this.Key="";
        this.Parts="";
    }
}

} // end of namespace
