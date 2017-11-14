# IoT Magic Mirror-
Windows Core IoT Töfraspegill

# 07.11.2017 - Status
Í dag var rifið skjáinn í sundur, gamall HP skjár var fórnarlambið og er hann núna geymdur í skolanum þangað til þurft verður frekar á honum að halda...

Annars var lítið gert í verkefninu nema þá gera tölvuna ready fyrir vinnslu, uppfært var Visual Studio frá 2013 í 2017 þar sem að auðveldara er að vinna með UWP í 2015+. einnig var komið fyrir IOT dashboardinu í tölvunni.

Vesen var á IoT Raspberryinum þar sem að stýrikerfið virðist hafa crashað þannig að tíminn fór meira og minna í það að horfa á skólatölvuna vera að rembast við að uppfæra og downloada nýju stýrikerfi fyrir Piinn þannig að ég féll á tíma þar. 

Það sem hafði verið gert áður enn ekki loggað var bara einfaldlega smá fikt og research í microsoft API og gert basic deployment á appi fyrir IoT og var prukeyrt að keyra það með vefmyndavél og láta taka mynd ásamt því að keyra klukku þannig það er flott mál. 

# 08.11.2017 - Status
Meira research og skoðað hvernig skyldi sameina, búinn að búa til nokkur console öpp með hjálp https://docs.microsoft.com/en-us/azure/cognitive-services/face/face-api-how-to-topics/ uppá að skilja betur hvernig þetta virkar og hvernig ætti að implementa þetta í forritið sjálft. 

Eitt sem gæti verið mögulegur skellur er það að fría útgáfan leyfir aðeins 20 uppflettingum á mín, ég lenti í því þegar ég var að láta það læra betur ákveðin andlit þá hætti forritið vegna "Limit Reached" þannig  mikilvægt er að hafa í huga þegar kemur að implememnta þessu er að kalla ekki oftar í functionið sem mun kanna hver er tilstaðar oftar en 1x á mín, og mögulega hugsa þetta þannig að appið þurfi ekki að kalla nema 1x til að staðfesta án þess að láta loopa í öll face-ID en ég skoða það nánar síðar.

Frítíminn sem gefst í dag til að skoða þetta fer í að kynnast bara ferlinu og gera nokkur minni console öpp og samtvinna svo þekkinguna sem öðlast þar til að skila inn réttum upplýsingum í UWP appinu.

# 09.11.2017 - Status
Vesen... fastur núna með sömu milluna og eftir svona 30 mismunandi útfærslur lagast það ekki... hann kvartar að ég sé að reyna gera eitthvað sem ætti að vega á bakgrunssþræði á UI þræðinum en svo virðist sem UWP threading hafi ekki sama og t.d. WPF eða console  þar sem að ég gæti bara úthlutað þessu á nýjan þráð, hinsvegar er eitthvað sem heitir Task sem á að gera eitthvað við Thread poolið en eftir mikið af mismunandi tilraunum þá gengur það ekki...

Seinna update: Vandræðaleg þetta var bara einhver smá villa sem stal þarna þessum tíma af mér... núna þekkir hún mig og getur stofnað nýja en þarf að laga hvernig hún vistar þá og trainar sig til þekkingu.

# 10.11.2017 - Status
Jæja hann lærir núna ný andlit og þekkir þau síðan aftur, vegna þess að ég hef bara heimild fyrir 20 calls á mín þá get ég ekki látið myndavélina kanna andlitið t.d. á 2 sek fresti  því þá jafnvel þótt ég minnki þetta í að hann geri allt í einu calli þá samt eftir 40 sek væri það completað svo eins og er þetta að refresha andlitið á mínotu fresti þangað til ég læt vega geyma bara face ID locally  og reyna switcha þannig á milli. 

Athyglisverð villa... ég nota alltaf sömu aðferðina til að taka mynd, tek snapshot frá myndavélinni set í möppu og ef að vélin þekkir þig ekki býr hún þig til og tekur nokkrar myndir af þér til að læra hver þú ert og vistar og allt í góðu og hún skilar síðan nafni... hinsvegar ef hún er einfaldlega bara að facechecka og ég nota sömu aðferð til að taka eina mynd þá kvartar hún að mynd sé of smá...... 

# 12.11.2017 - Status
Að þekkja svipbrigði og þess háttar er búið að bæta við en hvernig verður unnið úr því á ég eftir að skoða, eins og er ennþá vesen með að þrátt fyrir að framkvæma þetta á sama máta kemur kvörtun (stundum) að image size sé of smá, þrátt fyrir að þegar ég skoða þetta þá er stærð myndarinnar alltaf í kringum 50-70kb, tók að vísu eftir því að þegar eina ljósið sem ég fékk var ljósið frá skjánum duttu myndirnar í 30kb og þá gerðist þetta mun oftar... spurning hvort að betri myndavél sem tekur í kjölfarið betri myndir myndi útrýma þessu vandamáli, eða hafa bara alltaf mjög upplýst í kringum sig... 

-.- prófaði að force-a myndirnar í stærri upplausn, virðist ekki hafa skemmt facedetectionið þrátt fyrir smá auka blur en... vandamál en tilstaðar, ég hef prófað að endurraða hvenær hún tekur mynda uppá ef hann er að lesa ókláraða mynd en svo virðist það ekki leysa vandamálið, þannig eins og er þekkir þetta fyrstu manneskjuna og switchar ekki yfir í aðra sem kemur í staðinn nema þú sért mjög heppinn... 

# 14.11.2017 - Status
Verkefnið uppfært í Lokaverkefni, nýtt git fyrir það, lítið gerðist í skólanum þar sem að vélin var 90 mín að gera sig ready með viðeigandi uppfærslum og stillingum og loks þegar þetta var keyrt afstað í skjánum þá er skjárinn takmarkuð við mjög láta upplausn sem skilar þessu ekki jafn flott og í 1920 x 1080 og einhverjar upplýsingar skila sér ekki á skjáinn þannig það þarf að endurhugsa aðeins hvernig því verður háttað.

Að hlaða upp designerinn er að taka skuggalegan tíma þannig ég eflaust reyni bara að kíkja á þetta eftir vinnu í kvöld...
