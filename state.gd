extends Node

var max_level = 1
var current_level = 1

func next_level():
  max_level = max(max_level, current_level + 1)
  current_level += 1
