#!/usr/bin/env python3
from random import randint
import uuid

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
        "sound.wav"
        "sound.mp3"
    ]
    
    quests = [
        "null",
        "quest1.xml"
        "quest2.xml"
        "quest3.xml"
        "quest4.xml"
    ]
    
    bools = [
        "true",
        "false"
    ]
    
    def generate_data(self, threshold):
        # generates a GUID
        for index in range(threshold):
            the_id = uuid.uuid1()
            speaker = get_random_element(names)
            entry = get_random_element(sentences)
            animation = get_random_element(animations)
            sound = get_random_element(sounds)
            quest = get_random_element(quests)
            response_required = get_random_element(bools)
            prompt_object = PromptObject(
                the_id,
                speaker,
                entry,
                animation,
                sound,
                quest,
                response_required
            )
            prompt_objects.append(prompt_object)
            
        for index in range(threshold*2):
            the_id = uuid.uuid1()
            entry = get_random_element(sentences)
            next_entry = prompt_objects[randint(len(prompt_objects)-1)].the_id
            response_object = PromptObject(
                the_id,
                entry,
                next_entry
            )
            response_objects.append(response_object)
            
        for prompt_object in prompt_objects:
            if prompt_object.response_required:
                num_responses = randint(3) + 1
                for index in range(num_responses):
                    the_id = uuid.uuid1()
                    from_id = prompt_object.the_id
                    to_id = response_objects[randint(len(response_objects)-1)].the_id
                    response_map_object = ResponseMapObject(
                        the_id,
                        from_id,
                        to_id
                    )
                    response_map_objects.append(response_map_object)
            
    def to_sql_file(self, output_file_path):
        with open(output_file_path, 'w') as file:
            for prompt in prompt_objects:
                file.write("{}\n".format(prompt.to_insert()))
                
            for response in response_objects:
                file.write("{}\n".format(response.to_insert()))
            
            for response_map in response_map_objects:
                file.write("{}\n".format(response_map.to_insert()))

    def get_random_element(self, input_list):
        return input_list[randint(len(input_list)-1)]

class PromptObject():
    def __init__(self, the_id, speaker, entry, animation, sound, quest, response_required):
        self.the_id = the_id
        self.speaker = speaker
        self.entry = entry
        self.animation = animation
        self.quest = quest
        self.response_required = response_required
        
    def to_insert(self):
        sql = "INSERT INTO Prompt "
        sql += "VALUES("
        sql += the_id + ", "
        sql += "\"" + speaker + "\", "
        sql += "\"" + entry + "\", "
        sql += "\"" + animation + "\", "
        sql += "\"" + sound + "\", "
        sql += response_required
        sql += ");"

class ResponseObject():
    def __init__(self, the_id, entry, next_entry):
        self.the_id = the_id
        self.entry = entry
        self.next_entry = next_entry
        
    def to_insert(self):
        sql = "INSERT INTO Response "
        sql += "VALUES("
        sql += the_id + ", "
        sql += "\"" + entry + "\", "
        sql += next_entry
        sql += ");"
        
class ResponseMapObject():
    def __init__(self, the_id, from_id, to_id):
        self.the_id = the_id
        self.from_id = from_id
        self.to_id = to_id
        
    def to_insert(self):
        sql = "INSERT INTO Response_Map "
        sql += "VALUES("
        sql += the_id + ", "
        sql += from_id + ", "
        sql += to_id
        sql += ");"