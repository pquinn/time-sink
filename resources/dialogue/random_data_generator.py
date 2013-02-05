#!/usr/bin/env python3
from random import randint
from data_structures import *
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
                "",
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

def main(argv):
    data_generator = DataGenerator()
    data_generator.generate_data(100)
    data_generator.to_sql_file(argv[0])


if  __name__ =='__main__':main(sys.argv[1:])