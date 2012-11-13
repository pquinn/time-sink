#!/usr/bin/env python3
from random import randint
import uuid, sys

# make object to hold dialogue
# write them to file in SQLite syntax

class DataGenerator:
    
    prompt_objects = []
    response_objects = []
    response_map_objects = []
    
    sentences = [
        "This is a sentence.",
        "This is another sentence.",
        "What even is this?",
        "Who knows anymore.",
        "My whole life is ridiculous, really.",
        "Why am I still doing this?",
        "I need more hobbies.",
        "Some day..."
    ]
    
    names = [
        "Steve",
        "Nick",
        "Phil",
        "Kyle",
        "Julie"
    ]
    
    animations = [
        "null",
        "animation1.gif",
        "animation2.gif",
        "animation3.gif"
    ]

    sounds = [
        "null",
        "sound.wav",
        "sound.mp3"
    ]
    
    quests = [
        "null",
        "quest1.xml",
        "quest2.xml",
        "quest3.xml",
        "quest4.xml"
    ]
    
    bools = [
        "1",
        "0"
    ]
    
    def generate_data(self, threshold):
        # generates a GUID
        for index in range(threshold):
            the_id = uuid.uuid1()
            speaker = self.get_random_element(self.names)
            entry = self.get_random_element(self.sentences)
            animation = self.get_random_element(self.animations)
            sound = self.get_random_element(self.sounds)
            quest = self.get_random_element(self.quests)
            response_required = self.get_random_element(self.bools)
            prompt_object = PromptObject(
                the_id,
                speaker,
                entry,
                animation,
                sound,
                quest,
                response_required
            )
            self.prompt_objects.append(prompt_object)
            
        for index in range(threshold*2):
            the_id = uuid.uuid1()
            entry = self.get_random_element(self.sentences)
            next_entry = self.prompt_objects[randint(0, len(self.prompt_objects)-1)].the_id
            response_object = ResponseObject(
                the_id,
                entry,
                next_entry
            )
            self.response_objects.append(response_object)
            
        for prompt_object in self.prompt_objects:
            if prompt_object.response_required == "1":
                num_responses = randint(1, 3)
                for index in range(num_responses):
                    the_id = uuid.uuid1()
                    from_id = prompt_object.the_id
                    to_id = self.response_objects[randint(0, len(self.response_objects)-1)].the_id
                    response_map_object = ResponseMapObject(
                        the_id,
                        from_id,
                        to_id
                    )
                    self.response_map_objects.append(response_map_object)
            
    def to_sql_file(self, output_file_path):
        with open(output_file_path, 'w') as file:
            file.write("{}\n".format("DELETE FROM Prompt;"))
            file.write("{}\n".format("DELETE FROM Response;"))
            file.write("{}\n".format("DELETE FROM Response_Map;"))
            for prompt in self.prompt_objects:
                file.write("{}\n".format(prompt.to_insert()))
                
            for response in self.response_objects:
                file.write("{}\n".format(response.to_insert()))
            
            for response_map in self.response_map_objects:
                file.write("{}\n".format(response_map.to_insert()))

    def get_random_element(self, input_list):
        return input_list[randint(0, len(input_list)-1)]

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
            self.response_required
            )
        return sql

class ResponseObject():
    def __init__(self, the_id, entry, next_entry):
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

def main(argv):
    data_generator = DataGenerator()
    data_generator.generate_data(100)
    data_generator.to_sql_file(argv[0])


if  __name__ =='__main__':main(sys.argv[1:])