# DLS - Dialogue & Variable System Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.8.0-alpha.1] - 2023-06-17

### Added

- Added XNode as the base node based editor.
- Added Custom Nodes for Dialogue, Dialogue Choice, ReferenceState, Variable.
- Added properties to BaseNode to work with parsing.
- Added Player/Npc Scripts to be used with the system.
- Added Unity's New Input system support.
- Added Prefabs for Dialogue and Choice Buttons.
- Added Variable system using Scriptable Objects.
- Added DialogueUi Script to handle all forms of dialogue handling and variables.
- Added Scriptable Objects for Dialogue Graph, Dialogue Interactions, Variables.
- Added Dialogue Interaction code to be able to navigate between different dialogue graphs.
- Added String Extension ParseObject from DLS Utilities Version: 0.9.0
- Added User Guide and Documentation.
- Added handling for multiple edge cases when working with Dialogues and Variables.

## [0.8.1-alpha.1] - 2023-06-18

### Changed

- Changed hotkey to select all nodes to F3 from A
- Changed the following nodes: Dialogue, ChoiceDialogue, VariableNode so that
the text area expands and is more form fitting.