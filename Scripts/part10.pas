PROGRAM Part10;
VAR
   a, b : INTEGER;
   y    : REAL;
   z    : REAL;

BEGIN {Part10AST}
   a := 2;
   b := 10 * a + 10 * a DIV 4;
   y := 20 / 7 + 3.14;
   z := y * 5 - a;
END.  {Part10AST}