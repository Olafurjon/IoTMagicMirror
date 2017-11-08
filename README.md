# VEF3B3U---Verkefni-4-
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
