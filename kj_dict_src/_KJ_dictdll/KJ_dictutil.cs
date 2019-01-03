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

// 多くなったら HYAM.Utilで分離すべきだろう
namespace HYAM.KJ_dict
{
    


    public struct Pair {
        public string head;
        public string tail;
    }
    
    public class StringDivider {
        
        private int dividePoint;
        private int wordLength;
        private string baseword;
        
        private Pair  pair ;
//        private string []  pair ;
                      
        // constructor
        public StringDivider(string baseword) {
            this.baseword = baseword;
            this.dividePoint = 0;
            this.wordLength = baseword.Length;

            pair = new Pair();
           // pair = new string [2];
        }
        
        public bool eof() {
            if(this.dividePoint > this.wordLength){
                return true;
            }
            return false;
        }

        public Pair  DivideForward() {
        //  ABCED
        //    nul + ABCDE
        //    A   +  BCDE
        //    AB  +   CDE
        //     :       :
            pair.head = baseword.Remove(this.dividePoint,
                                      this.wordLength - this.dividePoint );
            pair.tail = baseword.Substring(this.dividePoint,
                                         this.wordLength - this.dividePoint );

            this.dividePoint++;
            return pair;
        }

        public Pair  DivideBackward() {
        //  ABCED
        //    ABCDE  + nul 
        //    ABCD   +   E
        //    ABC    +  DE
        //     :       :
            pair.head = baseword.Remove(this.wordLength - this.dividePoint,
                                      this.dividePoint );
            pair.tail = baseword.Substring(this.wordLength - this.dividePoint,
                                         this.dividePoint );

            this.dividePoint++;
            return pair;
        }
        
        
    }

} // End namespace KJ_dict
