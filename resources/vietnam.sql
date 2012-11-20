Schema:
Entry:
ID - GUID (Varchar(40)), PK
Body - Text

Response:
ID - GUID (Varchar(40)), PK
Body - Text
Next_ID - GUID (Varchar(40)), FK: Entry

Response_Map:
ID - GUID(Int), PK
Entry_ID - GUID (Varchar(40)), FK: Entry
Response_ID - GUID (Varchar(40)), FK: Response



INSERT INTO Entry VALUES(“129cfb6c-32de-11e2-8539-109adda800ea”, “The first is the hardest, the second is the next, and so forth, until eventually it becomes less an exercise of will than a mechanical tick put in motion.  Each step you take away from your bed moves you further into a reality so conveniently cast aside by lack of consciousness.  As the face that stares back at you in the mirror slowly becomes recognizable, you are reminded of just how easy it is to lose yourself momentarily.  You remind yourself that now is not the time to stop and reflect, because nothing can lead you astray quicker than breaking routine.”);

INSERT INTO Response VALUES(“12a087c6-32de-11e2-914a-109adda800ea”, “continue...”, “12a08ac0-32de-11e2-8c3c-109adda800ea”);

INSERT INTO Response_Map VALUES(0, “129cfb6c-32de-11e2-8539-109adda800ea”,
“12a087c6-32de-11e2-914a-109adda800ea”);

INSERT INTO Entry VALUES(“12a08ac0-32de-11e2-8c3c-109adda800ea”, “When you make your way down the stairs, you’re reacquainted with moments of your life as they adorn the white-washed walls.  You follow the trail of white until it leads you to another familiar setting.  Your mother, father, and younger sister are all happily decorating the family table, eagerly awaiting your arrival.  A pile of mail sits unobtrusively atop a black countertop.”);

INSERT INTO Response VALUES(“12a08c8a-32de-11e2-a483-109adda800ea”, “continue...”, “12a08e42-32de-11e2-9961-109adda800ea”);

INSERT INTO Response_Map VALUES(1,
“12a08ac0-32de-11e2-8c3c-109adda800ea”,
“12a08c8a-32de-11e2-a483-109adda800ea”);

INSERT INTO Entry VALUES(“12a08e42-32de-11e2-9961-109adda800ea”, “Mother: You have a letter!\n\nShe hands you a white envelope conspicuously accented with the Harvard seal.\n\n Mother: Oh, I just hope you’ll be able to join your brother at college!”);

INSERT INTO Response VALUES(“12a0d4ae-32de-11e2-80b2-109adda800ea”, “Eager”, “12a093f6-32de-11e2-b00d-109adda800ea”);
	
INSERT INTO Entry VALUES(“12a093f6-32de-11e2-b00d-109adda800ea”, “You take the letter and open it.”);


INSERT INTO Response VALUES(“12a0d668-32de-11e2-9d15-109adda800ea”, “continue...”, “12a091c8-32de-11e2-9a34-109adda800ea”);

INSERT INTO Response_Map VALUES(2, “12a093f6-32de-11e2-b00d-109adda800ea”, “12a0d668-32de-11e2-9d15-109adda800ea”);

INSERT INTO Response VALUES(“12a0d85c-32de-11e2-a397-109adda800ea”, “Hesitant”, “12a095cc-32de-11e2-b11d-109adda800ea”);

INSERT INTO Entry VALUES(“12a095cc-32de-11e2-b11d-109adda800ea”, “You: I’m not quite finished with my routine, I’ll open it later.\n Mother: Nonsense!  We’re just dying to find out!”);

INSERT INTO Response VALUES(“12a0da28-32de-11e2-bd0d-109adda800ea”, “continue...”, “12a091c8-32de-11e2-9a34-109adda800ea”);

INSERT INTO Response_Map VALUES(3, “12a095cc-32de-11e2-b11d-109adda800ea”, “12a0da28-32de-11e2-bd0d-109adda800ea”);

INSERT INTO Response_Map VALUES(4, “12a08e42-32de-11e2-9961-109adda800ea”, “12a0d4ae-32de-11e2-80b2-109adda800ea”);
INSERT INTO Response_Map VALUES(5, “12a08e42-32de-11e2-9961-109adda800ea”, “12a0d85c-32de-11e2-a397-109adda800ea”);

INSERT INTO Entry VALUES(“12a091c8-32de-11e2-9a34-109adda800ea”, “You open the letter with biting anticipation.  You can feel the weight of your families intrigue push against your shoulders.  It takes everything you have to keep standing.  Ripping the seal empowers you to keep going.  Gently, you slide the paper out and begin to read.  Your expression tells them everything they need to know.  While their weight lifts from your shoulders, it is replaced by something heavier still.”);

INSERT INTO Response VALUES(“12a0dbcc-32de-11e2-b103-109adda800ea”, “continue...”, “12a0977a-32de-11e2-a9be-109adda800ea”);

INSERT INTO Response_Map VALUES(6, “12a091c8-32de-11e2-9a34-109adda800ea”, “12a0dbcc-32de-11e2-b103-109adda800ea”);

INSERT INTO Entry VALUES(“12a0977a-32de-11e2-a9be-109adda800ea”, “Mother: I’m sorry dear.  I know how much this meant to you.\n\nBut did it really?”);

INSERT INTO Response VALUES(“12a0dd82-32de-11e2-8c17-109adda800ea”, “continue...”, “12a09994-32de-11e2-890f-109adda800ea”);

INSERT INTO Response_Map VALUES(7,
“12a0977a-32de-11e2-a9be-109adda800ea”,
“12a0dd82-32de-11e2-8c17-109adda800ea”);

INSERT INTO Entry VALUES(“12a09994-32de-11e2-890f-109adda800ea”, “The tension in the room begins to dissipate as you take your seat at the table between your younger sister and the vacant chair once occupied by your older brother. As you finish your breakfast, your father reminds you to bring the trash out with you on your way to school. You grab the trash bag along with your things and head out the door.”);

INSERT INTO Response VALUES(“12a0df3e-32de-11e2-874d-109adda800ea”, “continue...”, “12a09b3a-32de-11e2-b9bc-109adda800ea”);

INSERT INTO Response_Map VALUES(8,
“12a09994-32de-11e2-890f-109adda800ea”,
“12a0df3e-32de-11e2-874d-109adda800ea”);

INSERT INTO Entry VALUES(“12a09b3a-32de-11e2-b9bc-109adda800ea”, “You arrive at the trash bin moments before the garbage truck pulls up to the curb in front of you. As you remove the lid from the pail, a uniformed man steps off of the back of the truck.\n\n
Garbage man: The world is ending.”);

INSERT INTO Response VALUES(“12a0e112-32de-11e2-a367-109adda800ea”, “Disconcerted”, "12a09cfa-32de-11e2-bc71-109adda800ea”);

INSERT INTO Entry VALUES("12a09cfa-32de-11e2-bc71-109adda800ea”, “You: Uh, what?\n”);

INSERT INTO Response VALUES("12a0e2b6-32de-11e2-b3ee-109adda800ea”, “continue...”, "12a09eb4-32de-11e2-8112-109adda800ea”);
	
INSERT INTO Response_Map VALUES(9,
"12a09cfa-32de-11e2-bc71-109adda800ea”,
"12a0e2b6-32de-11e2-b3ee-109adda800ea”);

INSERT INTO Response VALUES("12a0e45a-32de-11e2-b02b-109adda800ea”, “Intrigued”, "12a0a058-32de-11e2-869b-109adda800ea”);
	
INSERT INTO Entry VALUES("12a0a058-32de-11e2-869b-109adda800ea”, “You: Excuse me? What did you say?\n\nGarbage man: The world; it’s ending.\n\n”

INSERT INTO Response VALUES("12a0e608-32de-11e2-87ab-109adda800ea”, “Oh yeah? And what makes you say that?”, "12a09eb4-32de-11e2-8112-109adda800ea”);

INSERT INTO Response_Map VALUES(10,
"12a0a058-32de-11e2-869b-109adda800ea”,
"12a0e608-32de-11e2-87ab-109adda800ea”);

INSERT INTO Response_Map VALUES(11,
“12a09b3a-32de-11e2-b9bc-109adda800ea”,
“12a0e112-32de-11e2-a367-109adda800ea”);
INSERT INTO Response_Map VALUES(12,
“12a09b3a-32de-11e2-b9bc-109adda800ea”,
"12a0e45a-32de-11e2-b02b-109adda800ea”);

INSERT INTO Entry VALUES("12a09eb4-32de-11e2-8112-109adda800ea”, “Garbage man: You don’t see it?\n\nYou notice that the driver exits the truck and makes his way around to where you’re standing.\n\nDriver: Who are you talking to?\n\nYou look back to where you were previously speaking to the garbage man. He’s gone.”);

INSERT INTO Response VALUES("12a0e7a4-32de-11e2-b7b6-109adda800ea”, “...uh, no one.”, "12a0a206-32de-11e2-a5eb-109adda800ea”);

INSERT INTO Response_Map VALUES(13,
"12a09eb4-32de-11e2-8112-109adda800ea”,
"12a0e7a4-32de-11e2-b7b6-109adda800ea”);

INSERT INTO Entry VALUES("12a0a206-32de-11e2-a5eb-109adda800ea”, “Driver: Oh, okay. Here, let me take that for you.\n\nThe driver extends his hand to take your trash. Upon taking it, he turns to deposit it in the back of the truck. You look down into the open pail and notice a newspaper with the headline: \“Three Killed in Heist; Suspect Still On the Loose\”\n\n”);

INSERT INTO Response VALUES("12a0e946-32de-11e2-95b3-109adda800ea”, “continue...”, "12a0a3ac-32de-11e2-a074-109adda800ea”); 

INSERT INTO Response_Map VALUES(14,
"12a0a206-32de-11e2-a5eb-109adda800ea”,
"12a0e946-32de-11e2-95b3-109adda800ea”);

INSERT INTO Entry VALUES("12a0a3ac-32de-11e2-a074-109adda800ea”
“Driver: Well, have a nice day now.\n\nYou look up slightly startled.\n\nYou: You too.\n\n
You place the lid back on top of the pail as if that simple act could possibly erase what just occurred.”);

INSERT INTO Response VALUES("12a0eae8-32de-11e2-905d-109adda800ea”, “continue...”, "12a0a558-32de-11e2-b2ec-109adda800ea”);

INSERT INTO Response_Map VALUES(15,
"12a0a3ac-32de-11e2-a074-109adda800ea”,
"12a0eae8-32de-11e2-905d-109adda800ea”);

INSERT INTO Entry VALUES("12a0a558-32de-11e2-b2ec-109adda800ea”, “You begin the walk to your school. On the way through the door, you collide with Frank. It would appear as though he purposely entered your path. Annoyed, you continue on the way to your first class.\n\nIt feels like at least a small portion of your burden is lifted immediately upon breaking the plane of the door. It’s your hope that the vibrant color of the art room could exonerate you from the events that preceded.  A friendly face beckons from across the room, so you gravitate toward it.”);

INSERT INTO Response VALUES("12a0eca4-32de-11e2-9b3d-109adda800ea”, “continue...”, "12a0a71a-32de-11e2-bec3-109adda800ea”);

INSERT INTO Response_Map VALUES(16,
"12a0a558-32de-11e2-b2ec-109adda800ea”,
"12a0eca4-32de-11e2-9b3d-109adda800ea”);

INSERT INTO Entry VALUES("12a0a71a-32de-11e2-bec3-109adda800ea”, “Taylor: Hey.  Ready for another joyous day at Sacred Heart?\n\nYou: Hallelujah...\n\nYou shrug and laugh a little to yourself.  Taylor could always be counted on to break a somber mood.”);

INSERT INTO Response VALUES("12a0ee50-32de-11e2-9f8f-109adda800ea”, “continue...”,  “12a0a8be-32de-11e2-8a93-109adda800ea”);

INSERT INTO Response_Map VALUES(17,
"12a0a71a-32de-11e2-bec3-109adda800ea”,
"12a0ee50-32de-11e2-9f8f-109adda800ea”);

INSERT INTO Entry VALUES("12a0a8be-32de-11e2-8a93-109adda800ea”, “Taylor: Well it looks like Van Gogh over there brought his crazy with him today.  You know, I didn’t think anyone could go toe to toe with you in the eccentric department, but I think he has your number on this one. \n\nYou: I know Daniel seems a little weird, but I think he’s a far shot from crazy.  I don’t think either of us really know him well enough to make that claim.\n\nTaylor: You sure about that?”);

INSERT INTO Response VALUES("12a0effe-32de-11e2-809b-109adda800ea”, “continue...”, "12a0aa94-32de-11e2-8ab2-109adda800ea”);

INSERT INTO Response_Map VALUES(18,
"12a0a8be-32de-11e2-8a93-109adda800ea”,
"12a0effe-32de-11e2-809b-109adda800ea”);

INSERT INTO Entry VALUES("12a0aa94-32de-11e2-8ab2-109adda800ea”, “Heeding Taylor’s prompt, you look over to where Daniel sits.  Daniel appears to be swinging wildly at the canvas, each stroke creating an axe-hewn rift in the painting.  The canvas is filled with disturbing imagery.  A figure appears to be floating in the sky, while a servant class is depicted below, reaching towards the heavens.  Souls seem to be evacuating the bodies of certain individuals, while the rest lay on the ground, burnt or burning.”);

INSERT INTO Response VALUES("12a0f1a2-32de-11e2-b8b2-109adda800ea”, “I don’t even...”, "12a0ac80-32de-11e2-bf77-109adda800ea”);

INSERT INTO Response_Map VALUES(19,
"12a0aa94-32de-11e2-8ab2-109adda800ea”,
"12a0f1a2-32de-11e2-b8b2-109adda800ea”);

INSERT INTO Entry VALUES("12a0ac80-32de-11e2-bf77-109adda800ea”, “Taylor: Yeah...\n\n The class ends some time thereafter. On your way to your next class, you notice Frank giving Brian a hard time in the hallway.”);

INSERT INTO Response VALUES("12a0f33a-32de-11e2-acff-109adda800ea”, “Intervene”, "12a0ae36-32de-11e2-9b1c-109adda800ea”);
	
INSERT INTO Entry VALUES("12a0ae36-32de-11e2-9b1c-109adda800ea”, “You: Hey! Leave him alone.\n\nFrank: Keep walking, dipshit.\n\nYou: I said leave him alone.\n\nFrank: If you don’t keep walking, we’re gonna have problems here.\n\n”);

INSERT INTO Response VALUES("12a0f4e2-32de-11e2-a196-109adda800ea”, “Listen, jackass, I’m not in the mood to deal with this right now. Just go to class.”, "12a0afda-32de-11e2-afdf-109adda800ea”);

INSERT INTO Response_Map VALUES(20,
"12a0ae36-32de-11e2-9b1c-109adda800ea”,
"12a0f4e2-32de-11e2-a196-109adda800ea”);

INSERT INTO Entry VALUES("12a0afda-32de-11e2-afdf-109adda800ea”, “Frank, infuriated by your comment , grabs you by your shirt and throws you against a locker.  There’s a loud ringing in your ear and the world gets a bit blurry.  A nearby teacher hears the noise and comes over to break up the trouble.\n\nMr. Marsden:  Hey, are you alright?”);

INSERT INTO Response VALUES("12a0f690-32de-11e2-ae55-109adda800ea”. “continue...”, "12a0b17e-32de-11e2-bde3-109adda800ea”);

INSERT INTO Response_Map VALUES(21,
"12a0afda-32de-11e2-afdf-109adda800ea”,
"12a0f690-32de-11e2-ae55-109adda800ea”);
	
INSERT INTO Entry VALUES("12a0b17e-32de-11e2-bde3-109adda800ea”, “The ringing has yet to subside.  In the corner of your eye you see a blurred image of Frank walking away, yet something about him seems peculiar.  For just a moment, you could have sworn you saw spikes protruding from his leather jacket.  Upon second glance you see nothing out of the ordinary.”);
	
INSERT INTO Response VALUES("12a0f828-32de-11e2-812a-109adda800ea”, “I’m fine.”, "12a0b340-32de-11e2-b53a-109adda800ea”);

INSERT INTO Entry VALUES("12a0b340-32de-11e2-b53a-109adda800ea”, “You: Don’t worry about it, I’m fine.\n\nYou take a step away from the teacher, but doing so causes you to lose your balance.  You catch yourself before falling, but even with your most graceful attempt, you’re not fooling anyone.\n\nMr. Marsden: Yeah, I don’t think so, let’s get you to the nurse.”

INSERT INTO Response VALUES("12a0f9de-32de-11e2-802a-109adda800ea”, “continue...”, "12a0b4ee-32de-11e2-ba57-109adda800ea”);

INSERT INTO Response_Map VALUES(22, "12a0b340-32de-11e2-b53a-109adda800ea”, "12a0f9de-32de-11e2-802a-109adda800ea”);

INSERT INTO Response VALUES("12a0fb90-32de-11e2-88e9-109adda800ea”, “I’m not so hot.”, "12a0b69c-32de-11e2-8e85-109adda800ea”);

INSERT INTO Entry VALUES("12a0b69c-32de-11e2-8e85-109adda800ea”, “You: I feel a little dizzy, maybe I should go lie down.\n\nYou take a step forward, but doing so causes you to lose your balance.  You catch yourself before falling.\n\nMr. Marsden: Let’s get you to the nurse.”

INSERT INTO Response VALUES("12a0fd28-32de-11e2-8bbc-109adda800ea”, “continue...”, "12a0b4ee-32de-11e2-ba57-109adda800ea”);

INSERT INTO Response_Map VALUES(23, "12a0b69c-32de-11e2-8e85-109adda800ea”, "12a0fd28-32de-11e2-8bbc-109adda800ea”);

INSERT INTO Response_Map VALUES(24, "12a0b17e-32de-11e2-bde3-109adda800ea”,
"12a0f828-32de-11e2-812a-109adda800ea”);

INSERT INTO Response_Map VALUES(25, "12a0b17e-32de-11e2-bde3-109adda800ea”, "12a0fb90-32de-11e2-88e9-109adda800ea”);

INSERT INTO Entry VALUES("12a0b4ee-32de-11e2-ba57-109adda800ea”, “While lying in the nurse’s office, Brian comes in to visit you.\nBrian: I hope you’re alright.\nYou: Yeah, I’m fine.\nBrian: Thanks for helping me. You didn’t have to do that.\nYou: Don’t worry about it. That guy is a jerk; he’ll get his.”);

INSERT INTO Response VALUES("12a0ff12-32de-11e2-b296-109adda800ea”, “Don’t worry about it. That guy is a jerk; he’ll get his.”, "12a0b840-32de-11e2-b8f4-109adda800ea”);

INSERT INTO Response_Map VALUES(26, "12a0b4ee-32de-11e2-ba57-109adda800ea”, "12a0ff12-32de-11e2-b296-109adda800ea”);

INSERT INTO Response VALUES("12a100c0-32de-11e2-86c8-109adda800ea”, “Overlook”, "12a0b9ee-32de-11e2-a1fc-109adda800ea”);

INSERT INTO Entry VALUES("12a0b9ee-32de-11e2-a1fc-109adda800ea”, “You feel compelled to intervene; however, you decide that it’s in your best interest to continue walking. As you pass, you hear the sound of someone hitting a locker and a grunt, followed by the scamper of a nearby witness and the subsequent reprimanding. A few minutes pass and you decide to go to the nurse’s office to visit Brian. Brian is holding some gauze under his nose to combat the bleeding. There’s dried blood above his upper lip.”);

INSERT INTO Response VALUES("12a10264-32de-11e2-bbad-109adda800ea”, “Are you alright?”, "12a0bb9c-32de-11e2-ad56-109adda800ea”);

INSERT INTO Response_Map VALUES(27, "12a0b9ee-32de-11e2-a1fc-109adda800ea”,
"12a10264-32de-11e2-bbad-109adda800ea”);	

INSERT INTO Entry VALUES("12a0bb9c-32de-11e2-ad56-109adda800ea”, “Brian: Yeah, I’ll be okay.”);

INSERT INTO Response VALUES("12a10412-32de-11e2-bad7-109adda800ea”, “continue...”, "12a0bd4a-32de-11e2-a8c1-109adda800ea”);

INSERT INTO Response_Map VALUES(28, "12a0bb9c-32de-11e2-ad56-109adda800ea”, "12a10412-32de-11e2-bad7-109adda800ea”);

INSERT INTO Entry VALUES("12a0bd4a-32de-11e2-a8c1-109adda800ea”, “While conversing with Brian, you hear what you think is the sound of footsteps and chains being dragged down the hallway towards the doorway. When you turn your head to observe what’s passing, you see nothing but the principal walking with Frank down the hallway. Your attention turns back to Brian.  You can see a flash of anger in his eyes as his gaze shifts back to you.”);

INSERT INTO Response VALUES("12a105b6-32de-11e2-8879-109adda800ea”, “continue...”, "12a0bef6-32de-11e2-8422-109adda800ea”);

INSERT INTO Response_Map VALUES(29, "12a0bd4a-32de-11e2-a8c1-109adda800ea”, "12a105b6-32de-11e2-8879-109adda800ea”);

INSERT INTO Entry VALUES("12a0bef6-32de-11e2-8422-109adda800ea”, “You: I’m sorry I didn’t step in.\n\nBrian: Whatever, it wasn’t your problem anyway. You’re no worse than anyone else that walked past, I guess.\n\nYou:  Yeah, but that doesn’t make it acceptable.  If everyone just sits around and...\n\nBrian: Forget it.  Just leave me alone.”);

INSERT INTO Response VALUES("12a1075a-32de-11e2-a892-109adda800ea”, “Alright...”, "12a0b840-32de-11e2-b8f4-109adda800ea”);

INSERT INTO Response_Map VALUES(46, "12a0bef6-32de-11e2-8422-109adda800ea”, "12a1075a-32de-11e2-a892-109adda800ea”);

INSERT INTO Response_Map VALUES(30, "12a0ac80-32de-11e2-bf77-109adda800ea”, "12a0f33a-32de-11e2-acff-109adda800ea”);

INSERT INTO Response_Map VALUES(31,
"12a0ac80-32de-11e2-bf77-109adda800ea”,
"12a100c0-32de-11e2-86c8-109adda800ea”);

INSERT INTO Entry VALUES("12a0b840-32de-11e2-b8f4-109adda800ea”, “The day progresses like any other, and the time comes to break for lunch. You exit the school with the intention of heading to the spot just past the dumpsters and out of the view of the faculty. A quick drag seems like just what you need right now. As you make your way past the dumpsters, to your dismay, you notice a familiar garbage truck.\n\n
Voice:  Do you see it yet?”);

INSERT INTO Response VALUES("12a10900-32de-11e2-8717-109adda800ea”, “continue...”, "12a0c0a6-32de-11e2-ab3f-109adda800ea”);

INSERT INTO Response_Map VALUES(32, "12a0b840-32de-11e2-b8f4-109adda800ea”, "12a10900-32de-11e2-8717-109adda800ea”);

INSERT INTO Entry VALUES("12a0c0a6-32de-11e2-ab3f-109adda800ea”, “You spin around quickly.  The world blurs and the line between reality and unconsciousness breaks down.\n\nYou: Who are you?\n\nGarbage Man: Do you see it yet?\n\nYou: Do I see what? What am I supposed to see?!\n\nHis face seems to move in and out of focus.”);

INSERT INTO Response VALUES("12a10ae6-32de-11e2-b37d-109adda800ea”, “continue...”, "12a0c25c-32de-11e2-a25e-109adda800ea”);

INSERT INTO Response_Map VALUES(33, "12a0c0a6-32de-11e2-ab3f-109adda800ea”, "12a10ae6-32de-11e2-b37d-109adda800ea”);

INSERT INTO Entry VALUES("12a0c25c-32de-11e2-a25e-109adda800ea”, “Garbage Man: It’s all around you, fetid and festering.\n\nYou: That’s disgusting, leave me alone.\n\nGarbage Man: The world is a disgusting place.  That’s why it has to end.\n\nYour head begins to hurt.  Putting a hand to your temple, you can feel it pulsating.”);

INSERT INTO Response VALUES("12a10c8a-32de-11e2-aa63-109adda800ea”, “Yeah, about that.  You’re insane.”, "12a0c400-32de-11e2-a8a8-109adda800ea”);

INSERT INTO Response_Map VALUES(34, "12a0c25c-32de-11e2-a25e-109adda800ea”, "12a10c8a-32de-11e2-aa63-109adda800ea”);

INSERT INTO Entry VALUES("12a0c400-32de-11e2-a8a8-109adda800ea”, “Garbage Man: Insanity is relative to your perception.\n\nYou:  Well we clearly don’t perceive things quite so similarly.\n\nGarbage Man: Don’t be so sure.”);

INSERT INTO Response VALUES("12a10e6e-32de-11e2-a324-109adda800ea”, “continue...”, "12a0c5ba-32de-11e2-a26e-109adda800ea”);

INSERT INTO Response_Map VALUES(35, "12a0c400-32de-11e2-a8a8-109adda800ea”, "12a10e6e-32de-11e2-a324-109adda800ea”);

INSERT INTO Entry VALUES("12a0c5ba-32de-11e2-a26e-109adda800ea”, “Voice: I knew I’d find you back here!\n\nYou look back to Taylor walking toward you.\n\nTaylor:  Hey, are you okay?  You don’t look so good.\n\nYou:  Yeah well...\n\nYou look back to where the garbage man was standing.  Just like before, he is gone.  A shadow of doubt creeps into your mind.”);

INSERT INTO Response VALUES("12a11010-32de-11e2-9267-109adda800ea”, “Confide”, "12a0c774-32de-11e2-ae86-109adda800ea”);

INSERT INTO Entry VALUES("12a0c774-32de-11e2-ae86-109adda800ea”, “You: I think I’m losing it.\n\nTaylor:  How so?\n\nYou:  I don’t know.  I’m just off.\n\nTaylor:  I heard about that scuffle earlier. Is Frank getting to you?”);

INSERT INTO Response VALUES("12a111b4-32de-11e2-813f-109adda800ea”, “Yeah, maybe. I think I just need a cigarette.”, "12a0c922-32de-11e2-8d81-109adda800ea”);

INSERT INTO Response_Map VALUES(36, "12a0c774-32de-11e2-ae86-109adda800ea”, "12a111b4-32de-11e2-813f-109adda800ea”);

INSERT INTO Response VALUES("12a11362-32de-11e2-a10b-109adda800ea”
“Conceal”
, "12a0cacc-32de-11e2-a82b-109adda800ea”

INSERT INTO Entry VALUES("12a0cacc-32de-11e2-a82b-109adda800ea”, “You: Never mind, just let me smoke this cigarette then let’s get out of here.”);

INSERT INTO Response VALUES("12a11506-32de-11e2-8052-109adda800ea”, “continue...”, "12a0c922-32de-11e2-8d81-109adda800ea”);

INSERT INTO Response_Map VALUES(37, "12a0cacc-32de-11e2-a82b-109adda800ea”, "12a11506-32de-11e2-8052-109adda800ea”);

INSERT INTO Response_Map VALUES(38, "12a0c5ba-32de-11e2-a26e-109adda800ea”, "12a11010-32de-11e2-9267-109adda800ea”);

INSERT INTO Response_Map VALUES(39, "12a0c5ba-32de-11e2-a26e-109adda800ea”, "12a11362-32de-11e2-a10b-109adda800ea”);

INSERT INTO Entry VALUES("12a0c922-32de-11e2-8d81-109adda800ea”
“Taylor: Alright.  Well hurry up, we’ve got to get back to class.  Wouldn’t want to keep that prick Hayden waiting.”);

INSERT INTO Response VALUES("12a116b4-32de-11e2-923d-109adda800ea”, “continue...”, "12a0cc70-32de-11e2-b153-109adda800ea”);

INSERT INTO Response_Map VALUES(40, "12a0c922-32de-11e2-8d81-109adda800ea”, "12a116b4-32de-11e2-923d-109adda800ea”);

INSERT INTO Entry VALUES("12a0cc70-32de-11e2-b153-109adda800ea”, “You arrive to class and take a seat next to Taylor.  Mr. Hayden walks in, his demeanor as pleasant as ever.  You notice Mr. Hayden has his gaze fixed on Rosalin, a quiet, studious girl sitting in the back of the room.  The light from the outside world is creeping through the windows, but does not reach her desk.  She appears veiled in the darkness, as if attempting to hide her secrets in the shadows.  He addresses her:\n\n
Mr. Hayden: What, did I scare you away or something?”);

INSERT INTO Response VALUES("12a11850-32de-11e2-9b47-109adda800ea”, “continue...”, "12a0ce22-32de-11e2-88c0-109adda800ea”)

INSERT INTO Response_Map VALUES(41, "12a0cc70-32de-11e2-b153-109adda800ea”, "12a11850-32de-11e2-9b47-109adda800ea”);

INSERT INTO Entry VALUES("12a0ce22-32de-11e2-88c0-109adda800ea”, “He looks at the empty seat at the front of the classroom with an amused glint in his eye.  Her eyes are averted, her presence quietly withdrawn.  He does not seem to expect a response, and she does not seem willing to defy his expectations.  An unsettling feeling washes over the class, and for a moment, time is suspended.”);

INSERT INTO Response VALUES("12a119fa-32de-11e2-9919-109adda800ea”
“continue...”, "12a0cfc6-32de-11e2-bb23-109adda800ea”);

INSERT INTO Response_Map VALUES(42,
"12a0ce22-32de-11e2-88c0-109adda800ea”,
"12a119fa-32de-11e2-9919-109adda800ea”);

INSERT INTO Entry VALUES("12a0cfc6-32de-11e2-bb23-109adda800ea”, “Mr. Hayden: Well I hope you all had as good a weekend as I did.  But like all good things, it has to end, so open your books to page 63.  As you are well aware, today’s reading assignment was about a concept prevalent in Christ based religions, and thus the Middle Ages”);

INSERT INTO Response VALUES("12a11ba8-32de-11e2-a47d-109adda800ea”, “continue...”, "12a0d168-32de-11e2-9f98-109adda800ea”);

INSERT INTO Response_Map VALUES(43, "12a0cfc6-32de-11e2-bb23-109adda800ea”, "12a11ba8-32de-11e2-a47d-109adda800ea”);

INSERT INTO Entry VALUES("12a0d168-32de-11e2-9f98-109adda800ea”, “As he turns around to write on the board, your heart sinks to your stomach.  A long, thin tail appears to be sprouting from his lower back.  It slithers like a snake as it wraps around and down his leg.  You are paralyzed with fear.  What was once a hand holding a crisp, white piece of chalk is now a set of bleeding fingers.  Across the board he smears in bold, bloody letters the word ATONEMENT.  A spastic shock forces your chair back.  The disturbance catches the attention of Mr. Hayden and your classmates.  To your horror, his face has become disfigured beyond recognition.  The contortions look inhuman, demonic even.  He stares at you with hollow eyes, all semblance of humanity eradicated.”);

INSERT INTO Response VALUES("12a11d50-32de-11e2-a937-109adda800ea”, ”continue...”, "12a0d30a-32de-11e2-95b5-109adda800ea”);

INSERT INTO Response_Map VALUES(44, "12a0d168-32de-11e2-9f98-109adda800ea”, "12a11d50-32de-11e2-a937-109adda800ea”);

INSERT INTO Entry VALUES("12a0d30a-32de-11e2-95b5-109adda800ea”, “Mr. Hayden: Does this concept frighten you, child?\n\nHis voice reverberates through your mind as if it were coming from within.  Sharp pain in your temple causes you to moan and reach for your head.  Some of your classmates giggle, others sound taken aback by your sudden outburst, though none of them seem to be aware of the being that was once your teacher.”);