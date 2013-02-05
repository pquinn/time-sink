__author__ = 'phil'

class PromptObject():
    def __init__(self, the_id, speaker, entry, animation, sound, quest, response_required):
        self.the_id = the_id
        self.speaker = speaker
        self.entry = entry
        self.animation = animation
        self.sound = sound
        self.quest = quest
        self.response_required = response_required

    def to_insert(self):
        # "The sum of 1 + 2 is {0}".format(1+2)
        sql = "INSERT INTO Prompt VALUES(\"{0}\", \"{1}\", {2}, {3}, {4}, {5}, {6});".format(
            self.the_id,
            self.speaker,
            "null" if self.entry == "null" else "\"{0}\"".format(self.entry),
            "null" if self.animation == "null" else "\"{0}\"".format(self.animation),
            "null" if self.sound == "null" else "\"{0}\"".format(self.sound),
            "null" if self.quest == "null" else "\"{0}\"".format(self.quest),
            "1" if self.response_required else "0"
            )
        return sql

class ResponseObject():
    def __init__(self, originating_id, the_id, entry, next_entry):
        self.originating_id = originating_id
        self.the_id = the_id
        self.entry = entry
        self.next_entry = next_entry

    def to_insert(self):
        sql = "INSERT INTO Response VALUES(\"{0}\", \"{1}\", \"{2}\");".format(
            self.the_id,
            self.entry,
            self.next_entry
            )
        return sql

class ResponseMapObject():
    def __init__(self, the_id, from_id, to_id):
        self.the_id = the_id
        self.from_id = from_id
        self.to_id = to_id

    def to_insert(self):
        sql = "INSERT INTO Response_Map VALUES(\"{0}\", \"{1}\", \"{2}\");".format(
            self.the_id,
            self.from_id,
            self.to_id
            )
        return sql
