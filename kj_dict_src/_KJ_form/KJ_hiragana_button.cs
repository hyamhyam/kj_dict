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

public class Hiragana_Button_Form : System.Windows.Forms.Form
{

    private CharButton [] HiraganaButtonList  ;
    
    private System.Windows.Forms.Button exitButton;
    private System.Windows.Forms.Button backspaceButton;
    private System.Windows.Forms.Button clearButton;
    
    private System.Windows.Forms.RichTextBox richTextBox1;
    
    
    
    public KJ_form kj_form; 
    
    public Hiragana_Button_Form(KJ_form parent)
    {
        this.kj_form = parent;
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        
        const int HiraganaButtonNumMax  = 52;

        const int ButtonSize = 25;
        const int startColumn  = 10;
        const int startRow     = 10;

        const int leftMargin   = 10;
        const int topMargin    = 20;
        
        const int mainFormWidth    = 320;
        const int mainFormHight    = 350;
        
        int row    = startRow;
        int column = startColumn;

        this.exitButton = new System.Windows.Forms.Button();
        this.exitButton.Text = "close"; 
        this.exitButton.Size     = new System.Drawing.Size(40, 20);
        this.exitButton.Location = new System.Drawing.Point( mainFormWidth - 50,
                                                             topMargin );
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
        this.richTextBox1.Location = new System.Drawing.Point(leftMargin + startColumn, 
                                                              300);
        this.richTextBox1.Size = new System.Drawing.Size(mainFormWidth - 50, 25);
        this.richTextBox1.ReadOnly = true ;
        this.richTextBox1.BackColor  = Color.Silver ;
        this.Controls.Add(this.richTextBox1); 
        
         
        HiraganaButtonList = new CharButton[HiraganaButtonNumMax] ;
        
        for(int i = 0 ; i < HiraganaButtonNumMax; i++){
            HiraganaButtonList[i] = new CharButton();
            
            HiraganaButtonList[i].Size     =
                         new System.Drawing.Size(ButtonSize, ButtonSize);
            if(i!=50 && i!=51){
                HiraganaButtonList[i].Click += 
                         new System.EventHandler(this.Hiragana_Click);
            }
            this.Controls.Add( HiraganaButtonList[i] ); 
        }
        
        HiraganaButtonList[0].Text = "あ";
        HiraganaButtonList[1].Text = "い";
        HiraganaButtonList[2].Text = "う";
        HiraganaButtonList[3].Text = "え";
        HiraganaButtonList[4].Text = "お";
        HiraganaButtonList[5].Text = "か";
        HiraganaButtonList[6].Text = "き";
        HiraganaButtonList[7].Text = "く";
        HiraganaButtonList[8].Text = "け";
        HiraganaButtonList[9].Text = "こ";
        HiraganaButtonList[10].Text = "さ";
        HiraganaButtonList[11].Text = "し";
        HiraganaButtonList[12].Text = "す";
        HiraganaButtonList[13].Text = "せ";
        HiraganaButtonList[14].Text = "そ";
        HiraganaButtonList[15].Text = "た";
        HiraganaButtonList[16].Text = "ち";
        HiraganaButtonList[17].Text = "つ";
        HiraganaButtonList[18].Text = "て";
        HiraganaButtonList[19].Text = "と";
        HiraganaButtonList[20].Text = "な";
        HiraganaButtonList[21].Text = "に";
        HiraganaButtonList[22].Text = "ぬ";
        HiraganaButtonList[23].Text = "ね";
        HiraganaButtonList[24].Text = "の";
        HiraganaButtonList[25].Text = "は";
        HiraganaButtonList[26].Text = "ひ";
        HiraganaButtonList[27].Text = "ふ";
        HiraganaButtonList[28].Text = "へ";
        HiraganaButtonList[29].Text = "ほ";
        HiraganaButtonList[30].Text = "ま";
        HiraganaButtonList[31].Text = "み";
        HiraganaButtonList[32].Text = "む";
        HiraganaButtonList[33].Text = "め";
        HiraganaButtonList[34].Text = "も";
        HiraganaButtonList[35].Text = "や";
        HiraganaButtonList[36].Text = "ゆ";
        HiraganaButtonList[37].Text = "よ";
        for(int i = 0 ; i < 38; i++){
            HiraganaButtonList[i].Location =
                    new System.Drawing.Point( leftMargin+column, 
                                              topMargin + row );
            column += ButtonSize;
            if(i%5==4){
                row   += ButtonSize;
                column = startColumn;
            }
        }

        HiraganaButtonList[38].Text = "ら";
        HiraganaButtonList[39].Text = "り";
        HiraganaButtonList[40].Text = "る";
        HiraganaButtonList[41].Text = "れ";
        HiraganaButtonList[42].Text = "ろ";
        HiraganaButtonList[43].Text = "わ";
        HiraganaButtonList[44].Text = "を";
        HiraganaButtonList[45].Text = "ん";
        column = startColumn;
        row   += ButtonSize;
        for(int i = 38 ; i < 46; i++){
            HiraganaButtonList[i].Location = 
                   new System.Drawing.Point( leftMargin+column, topMargin + row );
            column += ButtonSize;
            if(i==42){
                row   += ButtonSize;
                column = startColumn;
            }
        }

        HiraganaButtonList[46].Text = "っ";
        HiraganaButtonList[47].Text = "ゃ";
        HiraganaButtonList[48].Text = "ゅ";
        HiraganaButtonList[49].Text = "ょ";
        column = startColumn + (ButtonSize * 6);
        row   = startRow;
        for(int i = 46 ; i < 50; i++){
            HiraganaButtonList[i].Location =
                  new System.Drawing.Point( leftMargin+column, 
                                            topMargin + row );
            column += ButtonSize;
            if(i==46){
                row   += ButtonSize;
                column = startColumn + (ButtonSize * 6);
            }
        }
        
        HiraganaButtonList[50].Text = "゛";
        HiraganaButtonList[51].Text = "゜";
        column = startColumn + (ButtonSize * 6);
        row   = startRow + (ButtonSize * 3);
        HiraganaButtonList[50].Location = new System.Drawing.Point( leftMargin+column, 
                                                                    topMargin + row );
        HiraganaButtonList[50].Click += new System.EventHandler(this.dakuten_Click);
        column += ButtonSize;
        HiraganaButtonList[51].Location = new System.Drawing.Point( leftMargin+column, 
                                                                    topMargin + row );
        HiraganaButtonList[51].Click += new System.EventHandler(this.handakuten_Click);
        

        HiraganaButtonList[45].Text = "ん";
        
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
        this.ClientSize = new System.Drawing.Size(mainFormWidth, mainFormHight);
        this.Name = "MainForm";
        this.Text = "Hiragana Input method";
        
        // フォームの位置を調整可能にする
        this.StartPosition = FormStartPosition.Manual;
        // 位置設定
        this.Left = this.kj_form.Right;
        this.Top  = this.kj_form.Top  ;
        
    }  // end of InitializeComponent
    

    private void Hiragana_Click(object sender, System.EventArgs e)
    {
        this.richTextBox1.Text += ((CharButton)sender).Text ;
        refreshString() ;
    }
    
    private void dakuten_Click(object sender, System.EventArgs e)
    {
        if (!richTextBox1.Text.Equals(""))
        {
            int last = this.richTextBox1.Text.Length - 1 ;
            Char lastChar = this.richTextBox1.Text[last];
            
            if(canbewithDakuten(lastChar)){
                this.richTextBox1.Text = MakeDakuten(this.richTextBox1.Text);
            }else{
                this.richTextBox1.Text += ((CharButton)sender).Text ;
            }
        }else{
            this.richTextBox1.Text = ((CharButton)sender).Text ;
        }
        refreshString() ;
    }
    
    private void handakuten_Click(object sender, System.EventArgs e)
    {
        if (!richTextBox1.Text.Equals(""))
        {
            int last = this.richTextBox1.Text.Length - 1 ;
            Char lastChar = this.richTextBox1.Text[last];
            
            if(canbewithHandakuten(lastChar)){
                this.richTextBox1.Text = MakeHandakuten(this.richTextBox1.Text);
            }else{
                this.richTextBox1.Text += ((CharButton)sender).Text ;
            }
        }else{
            this.richTextBox1.Text = ((CharButton)sender).Text ;
        }
        refreshString() ;
    }

    
    
    private void exit_Click(object sender, System.EventArgs e)
    {
        this.Close();
    }
    private void clear_Click(object sender, System.EventArgs e)
    {
        this.richTextBox1.Text = "";
        this.kj_form.inputArea.Text =  "";
    }
    
    private void bs_Click(object sender, System.EventArgs e)
    { 
        if(this.richTextBox1.Text.Length == 0){
            return;
        }
        
        // から１文字削除し、再表示
        this.richTextBox1.Text =
            this.richTextBox1.Text.Remove(this.richTextBox1.Text.Length - 1 , 1);

        refreshString() ;
    }
    
    //------------------------------------------------------------------------
    private void refreshString()
    {   
        if(this.kj_form != null){
            this.kj_form.inputArea.Text = this.richTextBox1.Text ;
            this.kj_form.form_search(  );
        }
    }
    //------------------------------------------------------------------------
    private bool canbewithDakuten(Char ch)
    {   
        if(ch=='か' || ch=='き' || ch=='く' || ch=='け' || ch=='こ' ||
           ch=='さ' || ch=='し' || ch=='す' || ch=='せ' || ch=='そ' || 
           ch=='た' || ch=='ち' || ch=='つ' || ch=='て' || ch=='と' ||
           ch=='は' || ch=='ひ' || ch=='ふ' || ch=='へ' || ch=='ほ'    )
        {
            return true ;  
        }
        return false ;  
    }
    private string MakeDakuten(string text)
    {   
        Char lastChar = text[text.Length - 1] ;
        int hangulCode = lastChar;
        hangulCode ++;
        
        text = text.Remove(text.Length - 1 , 1) + char.ToString(Convert.ToChar(hangulCode));
        return text;
    }
    //------------------------------------------------------------------------
    private bool canbewithHandakuten(Char ch)
    {   
        if(ch=='は' || ch=='ひ' || ch=='ふ' || ch=='へ' || ch=='ほ'  )
        {
            return true ;  
        }
        return false ;  
    }
    private string MakeHandakuten(string text)
    {   
        Char lastChar = text[text.Length - 1] ;
        int hangulCode = lastChar;
        hangulCode = hangulCode  + 2;
        
        text = text.Remove(text.Length - 1 , 1) + char.ToString(Convert.ToChar(hangulCode));
        return text;
    }
    //------------------------------------------------------------------------

}



} // end of namespace
