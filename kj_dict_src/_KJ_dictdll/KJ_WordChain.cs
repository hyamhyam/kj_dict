// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics ;

namespace HYAM.KJ_dict
{

    public class WordChain {
        private WordTable head;   
        private WordTable tail;   
        
        //--------------------------------------------
        // constructor
        public WordChain() {
            this.head=null;
            this.tail=null;
        }
        public WordChain(WordTable wt1) : this() {
            this.Add(wt1);
        }
        public WordChain(WordTable wt1, WordTable wt2 ) : this() {
            this.Add(wt1);
            this.Add(wt2);
        }

        //--------------------------------------------
        // property 
        //
        public WordTable Head  {
            get{ return head; }
            set{ head=value; }
        }
        public WordTable Tail  {
            get{ return tail; }
            set{ tail=value; }
        }
        
        // Chain中のTableの数
        public int Length  {
            get{ 
                 int length = 0;
                 WordTable wt=this.head;
                 while(wt!=null){
                     length++ ;
                     wt = wt.next;
                 }
                 return length;
            }
        }
        
        //--------------------------------------------
        //
        //  public method
        //

        //--------------------------------------------
        // chainが存在するか否か
        public bool Exist(){
            if(this.head==null){
                return false;
            }
            return true;
        }
        
        // Chainの最後に語を追加
        public void Add(WordTable wt){
            if(head==null){
                this.head=wt;
                this.tail=wt;
                return;
            }
            this.tail.next = wt;
            wt.prev = tail;
            this.tail = wt;
        }
        
        // Chainの最後にChainを追加
        public void Add(WordChain wc2){
            if(wc2==null){
                return; // 何もしない
            }
            if(head==null){
                this.head = wc2.head ;
                this.tail = wc2.tail ;
                return;
            }
            
            this.tail.next = wc2.head;
            wc2.head.prev  = this.tail;
            
            this.tail = wc2.tail ;
            
        }
        
        //----------------------------------------------
        //
        //          wt2
        //           |
        //           v
        //  ----wt1--------a---------b--------c-----
        //              (oldnext)
        public void Insert(WordTable wt1, WordTable wt2){
            if(wt1==null){
                this.head=wt2;
                this.tail=wt2;
                return;
            }
            if(wt1.next==null){
                Debug.Assert(this.tail==wt1);
                wt1.next=wt2;
                wt2.prev=wt1;
                wt2.next=null;
                this.tail=wt2;
                return;
            }
            WordTable oldnext=wt1.next;
            wt1.next=wt2;
            wt2.prev=wt1;
            wt2.next=oldnext;
            oldnext.prev=wt2;
        }
        //----------------------------------------------
        //
        //      wt2
        //       |
        //       v
        // -p----------wt1--------a---------b--------c-----
        // (oldprev)
        public void InsertBefore(WordTable wt1, WordTable wt2){
            if(wt1==null){
                this.head=wt2;
                this.tail=wt2;
                return;
            }
            if(wt1.prev==null){
                Debug.Assert(this.head==wt1);
                this.head=wt2;
                wt2.next = wt1;
                wt2.prev = null;
                wt1.prev = wt2;
                return;
            }

            WordTable oldprev=wt1.prev;
            wt2.next=wt1;
            wt1.prev=wt2;
            wt2.prev=oldprev;
            oldprev.next=wt2;
        }
        
        
        //----------------------------------------------
        // チェーンの先頭にwtを入れる
        public void InsertHead(WordTable wt){
            if(this.head==null){
                this.head=wt;
                this.tail=wt;
                return;
            }
            WordTable oldHead = this.head;
            this.head = wt;
            wt.next = oldHead;
            oldHead.prev = wt;
        }
        //----------------------------------------------
        public void Delete(WordTable wt){
            WordTable w1=wt.prev;
            WordTable w2=wt.next;
            if(w1==null && w2 ==null){
                this.head=null;
                this.tail=null;
                wt=null;
                return;
            }
            if(w1==null){
                this.head=w2;
                w2.prev=null;
                wt=null;
                return;
            }
            if(w2==null){
                this.tail=w1;
                w1.next=null;
                wt=null;
                return;
            }
            
            w1.next=w2;
            w2.prev=w1;
            wt=null;
        }
        
        //----------------------------------------------
        //      
        //  chain中のwt1をwt2に置き換える
        //              
        public void Swap(WordTable wt1, WordTable wt2){
            Insert(wt1, wt2);
            Delete(wt1);
        }
        //----------------------------------------------
        //      
        //  chain中のwt1を wt2+wt3に置き換える
        //              
        public void Swap(WordTable wt1, WordTable wt2, WordTable wt3){
            Insert(wt1, wt2);
            Insert(wt2, wt3);
            Delete(wt1);
        }
        
        //----------------------------------------------
        //      
        //  chain中のwt1を別chainで置き換える
        //
        //      wc( wt2----------wt3---------wt4 )
        //      |   wc.head                  wc.tail
        //      v
        //  ----wt1--------a---------b--------c-----
        //              (oldnext)
        //              
        public void Swap(WordTable wt1, WordChain wc){
            Insert(wt1, wc);
            Delete(wt1);
        }
        
        // Insert chain 
        //         wc( wt2---wt3---wt4 )     
        //              
        //  ----wt1---wt2---wt3---wt4------a----b----c-----
        //              
        public void Insert(WordTable wt1, WordChain wc){
            if(wt1==null){
                this.head=wc.head;
                this.tail=wc.tail;
                return;
            }
            if(wc==null || wc.head==null){
                return;
            }
            if(wt1.next==null){
                Debug.Assert(this.tail==wt1);
                wt1.next=wc.head;
                wc.head.prev=wt1;
                this.tail=wc.tail;
                return;
            }
            
            WordTable oldnext=wt1.next;
            wt1.next=wc.head;
            wc.head.prev=wt1;
            wc.tail.next=oldnext;
            oldnext.prev=wc.tail;
        }
        
        //------------------------------------------------------------------
        public int GetChainCost()
        {
            int totalCost=0;
            WordTable wt=this.head;
            while(wt!=null){
                totalCost += wt.GetWordCost();
                wt = wt.next;
            }
            return totalCost;
        }    
        //------------------------------------------------------------------
        // Chain中に１つでも翻訳済みWordTableがあるならtrue
        // 全く翻訳できていないならfalse
        public bool IsTranslated()
        {
            WordTable wt=this.head;
            while(wt!=null){
                if(wt.IsTranslated()){
                    return true;
                }
                wt = wt.next;
            }
            return false;
        }    
        //------------------------------------------------------------------
        // Chain中の何文字翻訳できているか
        public int TranslatedCharCount()
        {
            WordTable wt=this.head;
            int rtnCnt=0;
            while(wt!=null){
                if(wt.IsTranslated()){
                    rtnCnt += wt.word.Length;
                }
                wt = wt.next;
            }
            return rtnCnt;
        }    
        //------------------------------------------------------------------
        // Chain中が全て翻訳済みWordTableならtrue
        // 1つでも翻訳できていないならfalse
        public bool IsFullTranslated()
        {
            WordTable wt=this.head;
            while(wt!=null){
                if(!wt.IsTranslated()){
                    return false;
                }
                wt = wt.next;
            }
            return true;
        }    

        //--------------------------------------------
        //
        //  static public method
        //
        static public WordChain GetMinimunCostChain(WordChain wc1, 
                                                    WordChain wc2   )
        {
            if(wc1==null){
                return wc2;
            }
            if(wc2==null){
                return wc1;
            }
            if(wc1.GetChainCost() < wc2.GetChainCost() ){
                return wc1;
            }else{
                return wc2;
            }
        }
        static public WordChain GetMinimunCostChain(WordChain wc1, 
                                                    WordChain wc2,
                                                    WordChain wc3   )
        {
            WordChain min_wc = GetMinimunCostChain(wc1, wc2);
            return GetMinimunCostChain(min_wc, wc3);
        }
        static public WordChain GetMinimunCostChain(WordChain wc1, 
                                                    WordChain wc2,
                                                    WordChain wc3, 
                                                    WordChain wc4   )
        {
            WordChain min_wc1 = GetMinimunCostChain(wc1, wc2);
            WordChain min_wc2 = GetMinimunCostChain(wc3, wc4);
            return GetMinimunCostChain(min_wc1, min_wc2);
        }
        static public WordChain GetMinimunCostChain(WordChain wc1, 
                                                    WordChain wc2,
                                                    WordChain wc3, 
                                                    WordChain wc4,   
                                                    WordChain wc5   )
        {
            WordChain min_wc1 = GetMinimunCostChain(wc1, wc2, wc3);
            WordChain min_wc2 = GetMinimunCostChain(wc4, wc5);
            return GetMinimunCostChain(min_wc1, min_wc2);
        }
            
    } // End class WordChain



} // End namespace KJ_dict
