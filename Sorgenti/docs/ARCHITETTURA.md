# Architettura di Anatomiva

## Dal prototipo all’app desktop

Anatomiva nasce dal progetto personale di Angela Plescia. Il [prototipo Python/Pygame](../legacy/) contiene la mappa interattiva e i quiz organizzati per parte del corpo. La versione desktop in C# estende quel percorso con un laboratorio del movimento, missioni guidate e un ripasso che usa gli esiti dei quiz.

Tre scelte guidano la versione desktop. La logica dei quiz e i calcoli biomeccanici sono separati dall’interfaccia, così si possono verificare con test dedicati. Le simulazioni usano modelli 2D per rendere leggibile il rapporto tra angolo, posizione e momento. I progressi vengono salvati sul PC e sono disponibili anche offline.

## Moduli separati
Il dominio in Core.cs e Learning.cs non dipende da Windows Forms. La UI usa quelle funzioni e mostra i risultati; i disegni non sono l'autorità per la valutazione. ProgressExport genera le righe CSV senza aprire finestre. Storage gestisce lettura, importazione e scrittura del profilo. App.cs e LearningPages.cs costruiscono le schermate; LabView.cs gestisce cursori, figure e animazione.

## Quiz e ripasso
QuizSession riceve cinque domande di un livello oppure fino a cinque domande del ripasso. Le opzioni hanno ID stabili e vengono mescolate una volta per domanda. Submit registra una sola risposta e apre il feedback; Continue passa alla domanda successiva o crea il risultato completo. Risultati parziali non sono salvati come quiz completati.

Il denominatore del punteggio dipende dalle domande effettive: un ripasso di due domande puo' terminare con 2/2. I quiz normali rimangono di cinque domande, con migliori punteggi distinti per categoria/livello. Ogni risultato conserva snapshot bilingui di domanda, risposta scelta, soluzione e spiegazione.

ReviewPlanner considera le risposte in ordine temporale e conserva l'ultima per ogni ID di domanda. Le domande con ultima risposta errata entrano nel ripasso; una risposta corretta successiva le rimuove. Vengono prima gli errori meno recenti. Il ripasso segue una regola deterministica. Una risposta nuovamente errata può far rientrare un concetto già ripassato.

## Laboratorio
LabModule definisce testo bilingue, intervallo del cursore, obiettivo e scelta concettuale corretta. LabState mantiene i parametri dell'esperimento. LabTrial deriva due condizioni: obiettivo numerico e concetto corretto; Passed richiede entrambe. La stessa verifica consecutiva, senza cambiare i parametri, riusa l'ID e non duplica il salvataggio. Dopo una modifica si può verificare di nuovo.

Elbow e knee usano un segmento rigido a lunghezza costante: endpoint relativo = (L sin θ, L cos θ), con y positivo verso il basso. Lo zero è l'estensione di riferimento. Ankle usa (L cos θ, L sin θ): zero con piede orizzontale, angolo negativo per dorsiflessione. Sono convenzioni grafiche dichiarate; non sono modelli completi delle articolazioni.

Nel laboratorio delle leve la forza del peso è verticale e il segmento orizzontale. Si usa il modulo del momento del solo carico, τ = m g d, con g = 9,81 m/s² e d in metri. La conversione da centimetri avviene prima del calcolo. Il grafico confronta il valore corrente con quello della stessa massa a 30 cm. Non si calcola la forza muscolare né si include il peso dell'arto.

Timer Windows Forms gestisce l'animazione: viene fermato prima della verifica e distrutto quando la schermata viene rimossa. Le frecce da tastiera e il mouse agiscono sullo stesso stato del modello.

## Archivio e compatibilità
Il nuovo archivio è XML schema 2 nella cartella utente Anatomiva. Un file temporaneo viene scritto e sottoposto a flush prima della sostituzione atomica, con backup del file precedente. In caso di errore, le aggiunte in memoria vengono annullate. Quiz, ripassi e prove del laboratorio hanno ID unici.

Al primo avvio è possibile importare lo schema 1 del vecchio profilo Body Explorer; il file originale non viene modificato. La copia viene scritta nel nuovo profilo in schema 2. Un profilo Anatomiva già presente non è mai sostituito dall'importazione. Versioni sconosciute dell'archivio o del modello di laboratorio vengono rifiutate. Per future modifiche delle regole di missione occorrerà versionare la valutazione, senza reinterpretare silenziosamente i dati esistenti.
