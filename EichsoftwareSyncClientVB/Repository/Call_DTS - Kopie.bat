Call GenerateFixSQLScriptMain^
 h2223265.stratoserver.net,^
 Herstellerersteichung,^
 Eichen,^
 Eichen2013,^
 WIN7MOBDEV01,^
 Herstellerersteichung,^
 sa,^
 Test1234,^
 1,^
 1,^
 3,^
 "Table_1,Benutzer,Firmen,ServerBeschaffenheitspruefung,Servereichmarkenverwaltung,ServerEichprotokoll,ServerEichprozess,ServerFirmenZusatzdaten,ServerKompatiblitaetsnachweis,ServerKonfiguration,ServerLizensierung,ServerLookup_Auswertegeraet,ServerLookup_Bearbeitungsstatus,ServerLookup_Konformitaetsbewertungsverfahren,ServerLookup_Vorgangsstatus,ServerLookup_Waagenart,ServerLookup_Waagentyp,ServerLookup_Waegezelle,ServerLookupVertragspartnerFirma,ServerMogelstatistik,ServerPruefungAnsprechvermoegen,ServerPruefungAussermittigeBelastung,ServerPruefungEichfehlergrenzen,ServerPruefungLinearitaetFallend,ServerPruefungLinearitaetSteigend,ServerPruefungRollendeLasten,ServerPruefungStabilitaetGleichgewichtslage,ServerPruefungStaffelverfahrenErsatzlast,ServerPruefungStaffelverfahrenNormallast,ServerPruefungWiederholbarkeit,ServerVerbindungsprotokoll"

Rem If error found while generating synchronization script then display error message and exit
IF ERRORLEVEL 1 (	echo Error found while generating synchronization script 
			echo.
			echo *** Log ****
			type OfflineSync.log
			exit /B %ERRORLEVEL%
		)

Rem If success then log script
echo Synchronization Script generated successfully
echo.
echo *** Synchronization Script ***
type FixSQLFile.sql
echo.
echo *** Log ****
type OfflineSync.log

Rem start synchronize
call offline_synchronization^
 DestinationServerName\SQLExpressDBServiceName,^
 DestinationDatabaseName,^
 DestinationDatabaseUserName,^
 DestinationDatabaseUserPassword,^
 1

Rem If error found then display error message and exit
IF ERRORLEVEL 1 (	echo Error found while synchronizing
			echo.
			echo *** Log ****
	 		type OfflineSync.log
	 		exit /B %ERRORLEVEL%
		)

Rem Return success
echo Synchronization done successfully
echo 0 & exit /B 

