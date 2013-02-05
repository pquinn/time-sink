INSERT INTO Prompt VALUES("c4aa9eba-6f3c-11e2-b9e5-109adda800ea", "speaker1", "this is the body of prompt 1", null, null, null, 1);
INSERT INTO Prompt VALUES("c4aaa005-6f3c-11e2-a966-109adda800ea", "speaker2", "this is actually prompt 2 now", null, null, null, 0);
INSERT INTO Prompt VALUES("c4aaa194-6f3c-11e2-80d9-109adda800ea", "speaker3", "and now this is prompt 3", null, null, null, 0);
INSERT INTO Response VALUES("c4a8ce28-6f3c-11e2-b75e-109adda800ea", "response 1 -> 2", "c4aaa005-6f3c-11e2-a966-109adda800ea");
INSERT INTO Response VALUES("c4aa9b9c-6f3c-11e2-9117-109adda800ea", "response 2 -> 2", "c4aaa005-6f3c-11e2-a966-109adda800ea");
INSERT INTO Response VALUES("c4aa9d0f-6f3c-11e2-b50d-109adda800ea", "response 3 -> 3", "c4aaa194-6f3c-11e2-80d9-109adda800ea");
INSERT INTO Response_Map VALUES("c4aaa2fd-6f3c-11e2-aa91-109adda800ea", "c4aa9eba-6f3c-11e2-b9e5-109adda800ea", "c4a8ce28-6f3c-11e2-b75e-109adda800ea");
INSERT INTO Response_Map VALUES("c4aaa421-6f3c-11e2-bb41-109adda800ea", "c4aa9eba-6f3c-11e2-b9e5-109adda800ea", "c4aa9b9c-6f3c-11e2-9117-109adda800ea");
INSERT INTO Response_Map VALUES("c4aaa538-6f3c-11e2-9fab-109adda800ea", "c4aa9eba-6f3c-11e2-b9e5-109adda800ea", "c4aa9d0f-6f3c-11e2-b50d-109adda800ea");
