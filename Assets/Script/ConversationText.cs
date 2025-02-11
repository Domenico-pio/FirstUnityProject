using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationText
{
    public bool attivo=false;       // Indicatore del se si sta mostrando il testo o meno 

    public GameObject go;           // GameObject della casella di testo contenente per l'appunto il testo
    public GameObject goSpeaker;    // GameObject della casella di testo contenente il nome di chi parla 

    public Text speaker;            // Nome di chi parla
    public string[] frasi;          // Lista di frasi che dirà
    public int fraseCorrente;       // Indice dell'ultima frase pronunciata (potrebbe diventare una variabile locale del metodo UpdateConversationText)
    public Text text;               // Testo pronunciato da chi parla

    public Vector3 motion;          // Vettore movimento del testo (da vedere se ha senso tenerlo)
    public Image sfondo;            // Immagine di sfondo al testo

    public void MostraTesto(){
        attivo = true;
        go.SetActive(attivo);
        sfondo.enabled=true;
        Time.timeScale=0;
    }

    public void NascondiTesto(){
        attivo = false;
        go.SetActive(attivo);
        sfondo.enabled=false;
        Time.timeScale=1;
    }

    // Per parlare con qulcuno si preme la barra spaziatrice, per andare avanti passando da una frase all'altra si preme la barra spaziatrice, se la frase è l'ultima si chiude la conversazione
    public void UpdateConversationText(){
        if (attivo){
            if (Input.GetKeyDown(KeyCode.Space)){   //Forse ha più senso metterlo nel ConversationTextManager
                if (fraseCorrente<frasi.Length)
                {
                    text.text=frasi[fraseCorrente];
                    fraseCorrente++;
                }
                else
                {
                    NascondiTesto();
                }
            }

            //totalmente inutile da quando ho messo timescale=0
            go.transform.position += motion*Time.deltaTime; // Se si imposta del movimento e il testo è attivo applichiamo il movimento
        }
        
    }
}
