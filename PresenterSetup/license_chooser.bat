REM A Tool for copying the correct license for Classroom Presenter
REM Input: The configuration string for the current build
@ECHO OFF

IF "%1"=="Debug" GOTO UW
IF "%1"=="Release" GOTO UW
IF "%1"=="RTPDebug" GOTO MSR
IF "%1"=="RTPRelease" GOTO MSR
GOTO END

:UW
ECHO Using UW License
copy ..\uw_license.rtf ..\license.rtf
GOTO END

:MSR
ECHO Using MSR License
copy ..\msr_license.rtf ..\license.rtf
GOTO END

:END
