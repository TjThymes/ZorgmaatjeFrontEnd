using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;

public class InformationManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI firstText;
    public TextMeshProUGUI secondText;
    public Image picture;

    private Dictionary<String, InformationData> informationData = new Dictionary<String, InformationData>();
    public void BackButtonClicked()
    {
        // Load the main menu scene
        SceneManager.LoadScene("HomeScene");
    }
    public Image KinderartsInfoImage, BeoordelingInfoImage, BloedonderzoekInfoImage, ThuisInfoImage, ZiekenhuisInfoImage, HerstelInfoImage, DagelijkslevenInfoImage;
    private void Start()
    {
        createInformation();
        var content = PlayerPrefs.GetString("SelectedContentID");
        title.text = informationData[content].title;
        firstText.text = informationData[content].firstText;
        secondText.text = informationData[content].secondText;
        picture.sprite = informationData[content].picture.sprite;
    }

    void createInformation()
    {
        informationData["Kinderarts"] = new InformationData(
            "Kinderarts", 
            "Welkom bij de kinderarts!\r\nVandaag ga je naar de kinderarts! Eerst moeten jij en je ouder(s) inchecken. Dat betekent dat je even je naam en afspraak doorgeeft. Vergeet ook niet om je verzekeringspasje en andere belangrijke papieren mee te nemen. Daarna wachten jullie in de wachtkamer totdat de kinderarts jullie komt halen. Spannend, maar geen zorgen, de dokter gaat je goed helpen!", "Wat neemt je ouder mee?\r\nJe ouder zorgt ervoor dat alles geregeld is! Ze nemen je verzekeringspasje mee en belangrijke papieren die de dokter nodig heeft. Zo kan de dokter precies zien wat er aan de hand is en wat er nodig is om je te helpen. Samen zorgen jullie ervoor dat alles goed gaat!",
            KinderartsInfoImage
        );
          informationData["Start"] = new InformationData(
            "Start", 
            "Welkom bij Zorgmaatje. Klik rustig door de tijdlijn heen en lees alles goed door. Veel leerplezier!",
            "Klik op Back om terug te gaan naar de tijdlijn.",
            KinderartsInfoImage
        );
        informationData["Beoordeling"] = new InformationData(
            "Eerste beoordeling",
            "Wat doet de dokter?\r\nDe dokter gaat met jou en je ouder(s) praten over hoe je je voelt. Misschien heb je vaak dorst, moet je veel plassen, ben je moe of ben je wat afgevallen. Dat zijn belangrijke dingen om te vertellen! De dokter wil graag weten wat er precies aan de hand is, zodat je de beste hulp krijgt", "Lichamelijk onderzoek\r\nNa het gesprek doet de dokter een klein onderzoek. Hij kijkt goed naar je lichaam om te controleren of er nog andere dingen zijn die met je klachten te maken kunnen hebben. Het is helemaal niet eng en de dokter vertelt steeds wat hij gaat doen. Zo weet je precies wat er gebeurt!",
            BeoordelingInfoImage
        );
        informationData["Bloedonderzoek"] = new InformationData(
            "Bloedonderzoek en diagnose",
            "Bloedonderzoek\r\nDe dokter gaat een klein beetje bloed afnemen om te kijken hoeveel suiker er in je bloed zit. Dit heet een bloedglucosemeting. Meestal doet de dokter dit door een klein prikje in je vinger. Het voelt even als een klein prikje, maar het doet niet veel pijn. De dokter gebruikt het bloed om te controleren hoe je lichaam met suiker omgaat.", "Wat betekent de uitslag?\r\nAls er veel suiker in je bloed zit, betekent dat misschien dat je diabetes type 1 hebt. De dokter legt dan goed uit wat dat precies betekent en wat er verder gaat gebeuren. Je bent niet alleen, er zijn veel kinderen die dit hebben, en de dokter gaat je helpen om je weer beter te voelen!",
            BloedonderzoekInfoImage
        );
        informationData["Thuis"] = new InformationData(
            "Thuisbehandeling",
            "De behandeling begint!\r\nDe dokter vertelt je dat je diabetes type 1 hebt. Dat kan best even schrikken zijn, maar je bent niet alleen! De dokter en een speciale diabetesverpleegkundige leggen alles rustig uit. Je leert hoe je insuline moet gebruiken, bijvoorbeeld met een prikje of een pompje. Ook leer je hoe je zelf je bloedsuiker kunt meten. Zo weet je wat je lichaam nodig heeft om zich goed te voelen.", "Weer naar huis!\r\nNa de uitleg mag je weer naar huis. Je krijgt een behandelplan mee waarin precies staat wat je elke dag moet doen. Soms moet je je bloedsuiker meten en insuline toedienen. Ook krijg je tips over wat je kunt eten en wat je beter kunt laten staan. Je ouders leren samen met jou hoe ze kunnen helpen. En als er iets is, kun je altijd bij de dokter of verpleegkundige terecht!",
            ThuisInfoImage
        );
        informationData["Ziekenhuis"] = new InformationData(
            "Ziekenhuisbehandeling",
            "Blijven in het ziekenhuis\r\nSoms moet je even in het ziekenhuis blijven als je je heel erg ziek voelt door diabetes. Dit gebeurt als je bijvoorbeeld veel dorst hebt, moe bent en je heel slap voelt. De dokter zegt dan dat je even moet blijven om goed in de gaten gehouden te worden. Je komt op de kinderafdeling, waar de dokters en verpleegkundigen extra goed op je letten. Ze geven je insuline en vocht via een infuus om je bloedsuiker weer normaal te maken.", "Weer naar huis\r\nAls de dokter ziet dat het beter met je gaat, mag je weer naar huis! Voordat je weggaat, leggen de verpleegkundige en de dokter uit wat je thuis moet doen. Ze vertellen je ouder(s) hoe ze je kunnen helpen en wat ze moeten doen als je je weer niet lekker voelt. Je krijgt een behandelplan mee met duidelijke uitleg. Zo weten jullie samen wat je elke dag kunt doen om je goed te voelen.",
            ZiekenhuisInfoImage
        );
        informationData["Herstel"] = new InformationData(
            "Herstel en controle",
            "Blijf in controle!\r\nAls je weer thuis bent, blijf je samen met je ouder(s) goed opletten hoe het met je gaat. Je hebt regelmatig afspraken bij de dokter om te controleren of je bloedsuiker goed blijft. Soms wordt er gekeken naar je ogen (retinopathie) of je zenuwen (neuropathie) om zeker te weten dat alles goed gaat. De dokter praat met jou en je ouder(s) over hoe het gaat en of er iets aan de behandeling moet worden aangepast.", "Steun van iedereen!\r\nHet is belangrijk dat je je gesteund voelt, zowel thuis als op school. Je kunt altijd praten over hoe je je voelt en wat je moeilijk vindt. Soms verandert de behandeling een beetje, bijvoorbeeld als je groeit of als je anders gaat eten of sporten. Samen met de dokter kijk je wat het beste bij jou past. En vergeet niet: je ouders, vrienden en dokters zijn er om je te helpen!",
            HerstelInfoImage
        );
        informationData["Dagelijksleven"] = new InformationData(
            "Dagelijks leven",
            "Weer naar school!\r\nJe mag gewoon weer naar school, maar soms moet je even je bloedsuiker meten of iets eten. Het is fijn als je leraar weet dat je diabetes hebt, zodat ze kunnen helpen als dat nodig is. Je neemt altijd een snack mee voor het geval je bloedsuiker te laag is. Samen met je ouders maak je afspraken met de school over wat er moet gebeuren als je je niet zo lekker voelt. Zo weet iedereen wat te doen!", "Zelfstandig en gezond!\r\nJe leert steeds beter om zelf je bloedsuiker te meten en insuline te geven. Dat voelt misschien een beetje spannend, maar je wordt er steeds beter in! Ook leer je welke dingen je beter wel en niet kunt eten. Bewegen en sporten is goed voor je, dus blijf vooral lekker actief. Heb je vragen of voel je je niet goed? Dan kun je altijd naar de diabetesverpleegkundige of het Diabetescentrum gaan.",
            DagelijkslevenInfoImage
        );

    }   
}
