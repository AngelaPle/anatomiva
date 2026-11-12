# Anatomiva 2.0.0 — test effettivamente eseguiti

Data: 5 novembre 2026. Host Ubuntu 24.04 x86_64, Mono 6.8.0.105, libgdiplus e Xvfb. Installer NSIS 3.09. Nessun ambiente Windows nativo disponibile: le verifiche seguenti non equivalgono a un collaudo sul PC dell'utente.

## Dominio, dati e formule

132 controlli automatici passati, con output in CORE_TEST_REPORT.txt. Comprendono:

- Catalogo di 12 categorie e 120 domande bilingui, opzioni valide e braccio distinto dall'orecchio.
- Punteggi perfetto, nullo e misto; protezione da doppie risposte; completamento e ordine stabile delle opzioni.
- Dati riaperti da disco, snapshot bilingui, idempotenza, backup, reset, file danneggiati e rollback dopo scrittura fallita.
- Momento esterno 1 kg a 30 cm = 2,943 N·m; proporzionalità con massa e distanza; ingresso non finito rifiutato.
- Posizione a zero e a 90° della cerniera; segno della dorsiflessione.
- Quattro missioni con condizioni numeriche e concettuali; tolleranza inclusiva ed esclusione del valore subito esterno; impossibilità di soddisfare la missione leve cambiando la massa richiesta.
- Prove di laboratorio salvate una volta sola, riapertura e rollback in caso di errore.
- Due errori entrano nel ripasso; risposta corretta li rimuove; nuovo errore rientra; massimo cinque domande per sessione; denominatore 2/2 e storico quiz conservato.
- CSV con colonne coerenti per quiz, ripasso e laboratorio.
- Importazione schema 1 nello schema 2 con originale intatto, punteggi conservati e nessuna sovrascrittura del nuovo profilo. Schema sconosciuto rifiutato.

## Interfaccia reale

Esecuzione della build con Mono/Xvfb, usando i controlli Windows Forms tramite Button.PerformClick, TrackBar, RadioButton e CheckBox:

- Home, laboratorio, quiz, risultati, ripasso, progressi, istruzioni e impostazioni aperti.
- Quattro missioni completate; prima prova numerica errata non accettata come successo.
- 24 flussi categoria/livello conclusi con punteggio controllato 3/5.
- Ripasso di due domande completato correttamente, con rimozione degli errori pendenti.
- Cambio lingua e testo grande a 1024×768; animazione avviata e fermata.
- Quiz, ripassi e cinque prove del laboratorio riaperti dall'archivio, più esportazione CSV con dati sintetici.
- Ispezione di catture reali. La misura del testo non ha rilevato overflow delle etichette nelle schermate campionate: LAYOUT_CHECK.txt.

Output: UI_TEST_REPORT.txt. Le immagini in screenshots/ mostrano dati sintetici del collaudo, non risultati utente; nessun archivio di test viene distribuito nel profilo dell'app.

## Riproduzione

Su Windows di sviluppo: packaging/build-windows.cmd compila e lancia i test di dominio. Per il collaudo grafico su una macchina di sviluppo con display attivo: `Anatomiva.exe --ui-test CARTELLA_VUOTA_ASSOLUTA`. Questa modalità usa un archivio isolato e crea dati sintetici nella cartella indicata.

Sotto Mono, dalla cartella del progetto:

```
mcs -r:System.Xml.Linq -resource:content/catalog.xml,catalog.xml -out:build/CoreTests.exe src/Core.cs src/Learning.cs src/ProgressExport.cs tests/CoreTests.cs
mono build/CoreTests.exe
```

La GUI richiede System.Windows.Forms, System.Drawing, System.Xml.Linq, catalog.xml e app.ico incorporati, un'icona Win32 e il manifest. Compilare tutti i file src/*.cs; la modalità grafica di test richiede un display. La compilazione di sviluppo supporta C# 5.

## Limiti delle verifiche

Installer compilato, con log in INSTALLER_BUILD_REPORT.txt; non eseguito su Windows. Non sono stati collaudati nativamente collegamenti desktop/Start, installazione/disinstallazione, controlli Windows sulla firma, configurazioni DPI o tecnologie assistive. Il dialogo di esportazione e i prompt di recupero/importazione sono implementati ma non automatizzati end-to-end. Nessuna validazione clinica o misura dell'efficacia didattica è dichiarata.
