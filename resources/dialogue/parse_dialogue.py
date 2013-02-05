__author__ = 'phil'
import uuid, shlex
from data_structures import *

class DialogueParser():

    prompt_objects = []
    response_objects = []
    response_map_objects = []

    def parse_lines(self, file_path):
        lines = []
        with open(file_path) as fd:
            for line in fd:
                lines.append(list(shlex.split(line)))
        return lines

    def create_dialogue_objects(self, lines):
        for line in lines:
            if line[0] == "prompt":
                #create prompt
                the_id = line[1]
                speaker = line[2]
                entry = line[3]
                animation = "null"
                sound = "null"
                quest = "null"
                response_required = True if line[4] == "yes" else False
                prompt_object = PromptObject(the_id, speaker, entry, animation, sound, quest, response_required)
                self.prompt_objects.append(prompt_object)
            elif line[0] == "response":
                #create a new id
                the_id = uuid.uuid1()
                originating_id = line[1]
                body = line[2]
                next_entry = line[3]
                response_object = ResponseObject(originating_id, the_id, body, next_entry)
                self.response_objects.append(response_object)

    def replace_ids(self):
        for prompt in self.prompt_objects:
            old_id = prompt.the_id
            new_id = uuid.uuid1()
            prompt.the_id = new_id
            for response in self.response_objects:
                if response.next_entry == old_id:
                    response.next_entry = new_id

                if response.originating_id == old_id:
                    response.originating_id = new_id

    def make_response_map_objects(self):
        for prompt in self.prompt_objects:
            prompt_id = prompt.the_id
            if prompt.response_required:
                for response in self.response_objects:
                    if response.originating_id == prompt.the_id:
                        the_id = uuid.uuid1()
                        from_id = prompt_id
                        to_id = response.the_id
                        response_map_object = ResponseMapObject(the_id, from_id, to_id)
                        self.response_map_objects.append(response_map_object)

    def to_sql_file(self, output_file_path, clear_tables_flag):
        with open(output_file_path, 'w') as file:
            if clear_tables_flag:
                file.write("{}\n".format("DELETE FROM Prompt;"))
                file.write("{}\n".format("DELETE FROM Response;"))
                file.write("{}\n".format("DELETE FROM Response_Map;"))

            for prompt in self.prompt_objects:
                file.write("{}\n".format(prompt.to_insert()))

            for response in self.response_objects:
                file.write("{}\n".format(response.to_insert()))

            for response_map in self.response_map_objects:
                file.write("{}\n".format(response_map.to_insert()))

if __name__ == '__main__':
    import sys
    file_path = sys.argv[1]
    output_file_path = sys.argv[2]
    dialogue_parser = DialogueParser()
    dialogue_parser.create_dialogue_objects(dialogue_parser.parse_lines(file_path))
    dialogue_parser.replace_ids()
    dialogue_parser.make_response_map_objects()
    dialogue_parser.to_sql_file(output_file_path, False)

  