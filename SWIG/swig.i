%module LogitechLCD

/* Parse the header file to generate wrappers */
%include <windows.i>
#define IN
#define OUT

%include "lglcd.h"

%{
/* Includes the header in the wrapper code */
#include "lglcd.h"
%}
