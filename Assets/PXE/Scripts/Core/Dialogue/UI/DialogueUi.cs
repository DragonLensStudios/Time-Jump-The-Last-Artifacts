using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Audio.Managers;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Dialogue.Managers;
using PXE.Core.Dialogue.Messaging.Messages;
using PXE.Core.Dialogue.Nodes;
using PXE.Core.Dialogue.Nodes.CustomNodes;
using PXE.Core.Enums;
using PXE.Core.Extensions.StringExtensions;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Core.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PXE.Core.Dialogue.UI
{
    //TODO: Refactor and remove normal gameobjects and use Object Controllers, and Also clean up and reuse as much code without duplication as possible.
    //TODO: Update Dialogue UI so that text is handled better and can be displayed in a more efficient way. with overflow handling.
    public class DialogueUi : ObjectController
    {
        public static DialogueUi Instance { get; private set; }

        [SerializeField] protected float typewriterTextDelayTime = 0.05f;

        [SerializeField] protected int maxDisplayedTextCharacters = 180;

        [SerializeField] protected List<string> displayedTextList;

        [SerializeField] protected TMP_Text actorNameUnderPortraitText, actorNameOverPortraitText, dialogueText;

        [SerializeField] protected Image actorImage;

        [SerializeField] protected GameObject backgroundActorNameUnderPortraitBox,
            backgroundActorNameOverDialogueBox,
            backgroundPortraitBox,
            dialogueBox,
            dialogueChoiceButtonPrefab,
            dialogueChoiceContainer,
            continueButton;

        [SerializeField] protected ChoiceDialogueNode activeSegment;
        [SerializeField] protected DialogueGraph selectedGraph;

        protected Coroutine _parser, _typeWriter;

        // protected EventSystem _eventSystem;
        protected PlayerInputActions _playerInput;

        protected ObjectController backgroundActorNameUnderPortraitBoxOc, backgroundActorNameOverDialogueBoxOc, backgroundPortraitBoxOc, dialogueBoxOc, dialogueChoiceContainerOc, continueButtonOc;

        protected bool _isDialogueOpen;

        protected bool isTypewriterRunning;

        public virtual bool IsDialogueOpen => _isDialogueOpen;
        public virtual float TypewriterTextDelayTime { get => typewriterTextDelayTime; set => typewriterTextDelayTime = value; }
        public virtual TMP_Text ActorNameUnderPortraitText { get => actorNameUnderPortraitText; set => actorNameUnderPortraitText = value; }
        public virtual TMP_Text ActorNameOverPortraitText { get => actorNameOverPortraitText; set => actorNameOverPortraitText = value; }
        public virtual TMP_Text DialogueText { get => dialogueText; set => dialogueText = value; }
        public virtual Image ActorImage { get => actorImage; set => actorImage = value; }
        public virtual GameObject BackgroundPortraitBox { get => backgroundPortraitBox; set => backgroundPortraitBox = value; }
        public virtual GameObject BackgroundActorNameUnderPortraitBox { get => backgroundActorNameUnderPortraitBox; set => backgroundActorNameUnderPortraitBox = value; }
        public virtual GameObject BackgroundActorNameOverDialogueBox { get => backgroundActorNameOverDialogueBox; set => backgroundActorNameOverDialogueBox = value; }
        public virtual GameObject DialogueBox { get => dialogueBox; set => dialogueBox = value; }
        public virtual GameObject DialogueChoiceButtonPrefab { get => dialogueChoiceButtonPrefab; set => dialogueChoiceButtonPrefab = value; }
        public virtual GameObject DialogueChoiceContainer { get => dialogueChoiceContainer; set => dialogueChoiceContainer = value; }
        public virtual ChoiceDialogueNode ActiveSegment { get => activeSegment; set => activeSegment = value; }
        public virtual DialogueGraph SelectedGraph { get => selectedGraph; set => selectedGraph = value; }


        public override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(obj: Instance);
            }
            else
            {
                // _eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
                Instance = this;
            }

            base.Awake();

            backgroundActorNameUnderPortraitBoxOc = backgroundActorNameUnderPortraitBox.GetComponent<ObjectController>();
            backgroundActorNameOverDialogueBoxOc = backgroundActorNameOverDialogueBox.GetComponent<ObjectController>();
            dialogueBoxOc = dialogueBox.GetComponent<ObjectController>();
            backgroundPortraitBoxOc = backgroundPortraitBox.GetComponent<ObjectController>();
            dialogueChoiceContainerOc = dialogueChoiceContainer.GetComponent<ObjectController>();
            continueButtonOc = continueButton.GetComponent<ObjectController>();
        }

        public override void OnActive()
        {
            base.OnActive();
            _playerInput ??= new PlayerInputActions();
            _playerInput.Enable();
            _playerInput.Player.Interact.performed += NextDialogue;
            MessageSystem.MessageManager.RegisterForChannel<DialogueMessage>(channel: MessageChannels.Dialogue, handler: DialogueMessageHandler);
        }


        public override void OnInactive()
        {
            base.OnInactive();
            _playerInput ??= new PlayerInputActions();
            _playerInput.Disable();
            _playerInput.Player.Interact.performed -= NextDialogue;
            MessageSystem.MessageManager.UnregisterForChannel<DialogueMessage>(channel: MessageChannels.Dialogue, handler: DialogueMessageHandler);
        }

        /// <summary>
        ///     Starts a dialogue with the provided <see cref="DialogueGraph" />
        /// </summary>
        /// <param name="graph"></param>
        public virtual void StartDialogue(DialogueGraph graph)
        {
            try
            {
                if (graph != null)
                {
                    selectedGraph = graph;
                }

                if (selectedGraph == null)
                {
                    return;
                }

                if (dialogueBoxOc != null)
                {
                    if (!dialogueBoxOc.IsActive)
                    {
                        dialogueBoxOc.SetObjectActive(active: !string.IsNullOrEmpty(value: dialogueText.text));
                    }
                }
                else
                {
                    if (!dialogueBox.activeSelf)
                    {
                        dialogueBox.SetActive(value: !string.IsNullOrEmpty(value: dialogueText.text));
                    }
                }

                if (continueButtonOc != null)
                {
                    continueButtonOc.SetObjectActive(active: false);
                }
                else
                {
                    continueButton.SetActive(value: false);
                }

                SetSelectedGameObject(go: continueButton);
                foreach (var node in selectedGraph.nodes)
                {
                    var b = (BaseNode)node;
                    if (b.GetNodeType() != nameof(StartNode))
                    {
                        continue; //"b" is a reference to whatever node it's found next. It's an enumerator variable 
                    }

                    selectedGraph.current = b; //Make this node the starting point. The [g] sets what graph to Use in the array OnTriggerEnter
                    break;
                }

                _parser = StartCoroutine(routine: ParseNode());
            }
            catch (Exception ex)
            {
                Debug.LogError(message: $"ERROR: Selected Graph Exception\n {ex.Message}");
            }
        }

        /// <summary>
        ///     Clears the Dialogue Choices.
        /// </summary>
        public virtual void ClearChoiceSelection()
        {
            if (!dialogueChoiceContainer.activeSelf)
            {
                return;
            }

            foreach (Transform child in dialogueChoiceContainer.transform)
            {
                Destroy(obj: child.gameObject);
            }
        }

        /// <summary>
        ///     This method updates the Choice Dialogue values and display.
        /// </summary>
        /// <param name="node"></param>
        public virtual IEnumerator UpdateDialogueChoices(ChoiceDialogueNode node)
        {
            activeSegment = node;
            StopTypeWriter();
            DisplayTextWithTypewriter(text: node.DialogueText.ParseObject(target: node), splitIntoChunks: true);
            // _typeWriter = StartCoroutine(TypewriterText(node.DialogueText.ParseObject(node), typewriterTextDelayTime, dialogueText));

            ClearChoiceSelection();
            var selectedIndex = 0;

            yield return new WaitUntil(predicate: () => dialogueText.maxVisibleCharacters >= node.DialogueText.ParseObject(target: node).Length);
            if (continueButtonOc != null)
            {
                continueButtonOc.SetObjectActive(active: false);
            }
            else
            {
                continueButton.SetActive(value: false);
            }

            foreach (var answer in node.Answers)
            {
                var btnObj = Instantiate(original: dialogueChoiceButtonPrefab, parent: dialogueChoiceContainer.transform); //spawns the buttons 
                btnObj.GetComponentInChildren<TMP_Text>().text = answer.ParseObject(target: node);
                var index = selectedIndex;
                var btn = btnObj.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    if (selectedIndex == 0)
                    {
                        SetSelectedGameObject(go: btnObj);
                    }

                    btn.onClick.AddListener(() => { AnswerClicked(clickedIndex: index); });
                }

                selectedIndex++;
            }
        }

        /// <summary>
        ///     This method gets the port for the Answers dialogue and sets the <see cref="selectedGraph" /> to that connection node.
        /// </summary>
        /// <param name="clickedIndex"></param>
        public virtual void AnswerClicked(int clickedIndex)
        {
            //Function when the choices button is pressed 
            if (dialogueChoiceContainerOc != null)
            {
                dialogueChoiceContainerOc.SetObjectActive(active: false);
            }
            else
            {
                dialogueChoiceContainer.SetActive(value: false);
            }

            var port = activeSegment.GetPort(fieldName: "Answers " + clickedIndex);

            if (port.IsConnected)
            {
                selectedGraph.current = port.Connection.node as BaseNode;
                _parser = StartCoroutine(routine: ParseNode());
            }
            else
            {
                if (dialogueBoxOc != null)
                {
                    dialogueBoxOc.SetObjectActive(active: false);
                }
                else
                {
                    dialogueBox.SetActive(value: false);
                }

                NextNode(fieldName: "input");
                Debug.LogError(message: "ERROR: ChoiceDialogue port is not connected");
                //NextNode("exit"); 
            }
        }

        public virtual IEnumerator TypewriterText(string text, float delay, TMP_Text outputText)
        {
            isTypewriterRunning = true;
            outputText.maxVisibleCharacters = 0;
            outputText.text = text;

            for (var i = 0; i < outputText.text.Length; i++)
            {
                outputText.maxVisibleCharacters++;
                yield return new WaitForSeconds(seconds: delay);
            }

            StopTypeWriter();
        }

        // public virtual IEnumerator TypewriterText(string text,float delay, TMP_Text outputText)
        // {
        //     outputText.maxVisibleCharacters = 0;
        //     outputText.text = text;
        //     for (int i = 0; i < text.Length; i++)
        //     {
        //         outputText.maxVisibleCharacters++;
        //         yield return new WaitForSeconds(delay);
        //     }
        //     var characterCount = outputText.textInfo.characterCount;
        //     bool isOverflow = outputText.isTextOverflowing;
        //     if (isOverflow)
        //     {
        //         Debug.Log(outputText.text[outputText.firstOverflowCharacterIndex..]);
        //     }
        //     Debug.Log($"Character Count: {characterCount} Overflow: {isOverflow}");
        //     StopTypeWriter();
        // }

        public virtual void StopTypeWriter()
        {
            if (_typeWriter != null)
            {
                StopCoroutine(routine: _typeWriter);
                _typeWriter = null;
            }

            if (continueButtonOc != null)
            {
                continueButtonOc.SetObjectActive(active: true);
            }
            else
            {
                continueButton.SetActive(value: true);
            }

            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            SetSelectedGameObject(go: continueButton);
            EventSystem.current.enabled = true;
            isTypewriterRunning = false;
        }

        /// <summary>
        ///     This method handles navigation to the next node based on the exit node string value.
        /// </summary>
        /// <param name="fieldName"></param>
        public virtual void NextNode(string fieldName)
        {
            actorNameUnderPortraitText.text = "";
            dialogueText.text = "";
            ClearChoiceSelection();
            if (_parser != null)
            {
                StopCoroutine(routine: _parser);
                _parser = null;
            }

            try
            {
                foreach (var p in selectedGraph.current.Ports)
                {
                    try
                    {
                        if (p.fieldName == fieldName)
                        {
                            selectedGraph.current = p.Connection.node as BaseNode;
                            break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogError(message: "ERROR: Port is not connected");
                    }
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogError(message: "ERROR: One of the elements on the Graph array at NodeParser is empty. Or, the Dialogue Graph is empty");
            }

            _parser = StartCoroutine(routine: ParseNode());
        }

        // TODO: Adjust this if needed. Added temporarily to make sure that interact buttons work with dialogue.
        public virtual void NextDialogue(InputAction.CallbackContext ctx)
        {
            NextDialogue();
        }

        public virtual void NextDialogue()
        {
            if (dialogueBoxOc != null)
            {
                if (!dialogueBoxOc.IsActive)
                {
                    return;
                }
            }
            else
            {
                if (!dialogueBox.activeSelf)
                {
                    return;
                }
            }

            var dialogueNode = selectedGraph.current as DialogueNode;
            var choiceNode = selectedGraph.current as ChoiceDialogueNode;

            if (dialogueNode != null || choiceNode != null)
            {
                if (isTypewriterRunning)
                {
                    StopTypeWriter();
                    dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                }
                else if (displayedTextList.Count > 0)
                {
                    StopTypeWriter();
                    DisplayTextWithTypewriter(text: displayedTextList[index: 0], splitIntoChunks: false);
                    displayedTextList.RemoveAt(index: 0);
                }
                else
                {
                    if (dialogueNode != null)
                    {
                        NextNode(fieldName: nameof(dialogueNode.Exit));
                    }
                    else if (choiceNode != null)
                    {
                        StartCoroutine(routine: UpdateDialogueChoices(node: choiceNode));
                    }
                }
            }
        }

        public virtual void DisplayTextWithTypewriter(string text, bool splitIntoChunks, int chunkSize = -1)
        {
            if (splitIntoChunks)
            {
                displayedTextList = SplitTextIntoChunks(text: text, maxChunkSize: chunkSize < 0 ? maxDisplayedTextCharacters : chunkSize);
                if (displayedTextList.Count > 0)
                {
                    _typeWriter = StartCoroutine(routine: TypewriterText(text: displayedTextList[index: 0], delay: typewriterTextDelayTime, outputText: dialogueText));
                    displayedTextList.RemoveAt(index: 0);
                }
            }
            else
            {
                _typeWriter = StartCoroutine(routine: TypewriterText(text: text, delay: typewriterTextDelayTime, outputText: dialogueText));
            }
        }

        public virtual List<string> SplitTextIntoChunks(string text, int maxChunkSize)
        {
            var chunks = new List<string>();
            var textLength = text.Length;
            var start = 0;

            while (start < textLength)
            {
                // Determine the end of the chunk
                var end = Mathf.Min(a: start + maxChunkSize, b: textLength);
                var adjustedEnd = end;

                // Check for ellipsis within the chunk
                var ellipsisIndex = text.IndexOf("...", start, StringComparison.Ordinal);
                if (ellipsisIndex != -1 && ellipsisIndex < end)
                {
                    end = ellipsisIndex + 3;
                }
                // If the end is not at the end of the text and the character at 'end' is not a sentence delimiter
                else if (end < textLength && !IsSentenceDelimiter(text: text, index: end, endIndex: out adjustedEnd))
                {
                    // Move the end back to the last sentence delimiter within the chunk
                    var lastSentenceDelimiter = LastIndexOfSentenceDelimiter(text: text, start: start, end: end, adjustedEnd: out adjustedEnd);
                    if (lastSentenceDelimiter > start)
                    {
                        end = adjustedEnd + 1;
                    }
                    else
                    {
                        // If no sentence delimiter is found, move the end back to the last space
                        var lastSpace = text.LastIndexOf(value: ' ', startIndex: end - 1, count: end - start);
                        if (lastSpace > start)
                        {
                            end = lastSpace;
                        }
                    }
                }
                else
                {
                    end = adjustedEnd + 1;
                }

                // Add the chunk to the list
                if (end > textLength)
                {
                    end = textLength;
                }

                chunks.Add(item: text.Substring(startIndex: start, length: end - start).Trim());

                // Move the start to the next character after the chunk
                start = end;

                // Skip any spaces, new lines, or tabs at the start of the next chunk
                while (start < textLength && char.IsWhiteSpace(c: text[index: start]))
                {
                    start++;
                }
            }

            return chunks;
        }

        public virtual int LastIndexOfSentenceDelimiter(string text, int start, int end, out int adjustedEnd)
        {
            adjustedEnd = end;
            for (var i = end - 1; i >= start; i--)
            {
                if (IsSentenceDelimiter(text: text, index: i, endIndex: out adjustedEnd))
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual bool IsSentenceDelimiter(string text, int index, out int endIndex)
        {
            endIndex = index;

            if (index < 0 || index >= text.Length)
            {
                return false;
            }

            // Check for standard sentence delimiters
            var c = text[index: index];
            if (c is '.' or '!' or '?')
            {
                // Handle ellipses with varying amounts of '.'
                if (c == '.')
                {
                    var periodCount = 0;
                    while (index + periodCount < text.Length && text[index: index + periodCount] == '.')
                    {
                        periodCount++;
                    }

                    // Consider as ellipsis if there are 3 or more periods
                    if (periodCount >= 3)
                    {
                        endIndex = index + periodCount - 1;
                        return true;
                    }
                }

                endIndex = index;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     This IEnumerator method handles each node and their actions.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator ParseNode()
        {
            var b = selectedGraph.current;
            var nodeName = b.GetNodeType();
            var targetActor = b.TargetActor;
            var sourceActor = b.SourceActor;
            var sourceGameObject = b.SourceGameobject;
            var targetGameObject = b.TargetGameobject;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearChoiceSelection();

            switch (nodeName)
            {
                case nameof(StartNode):
                    var sn = b as StartNode;
                    _isDialogueOpen = true;
                    NextNode(fieldName: nameof(sn.Exit));
                    break;
                case nameof(DialogueNode):
                    var dn = b as DialogueNode;
                    if (dn == null)
                    {
                        break;
                    }

                    if (dialogueBoxOc != null)
                    {
                        dialogueBoxOc.SetObjectActive(active: !string.IsNullOrEmpty(value: dn.DialogueText));
                    }
                    else
                    {
                        dialogueBox.SetActive(value: !string.IsNullOrEmpty(value: dn.DialogueText));
                    }

                    if (continueButtonOc != null)
                    {
                        continueButtonOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        continueButton.SetActive(value: false);
                    }

                    if (dn.UseSourceActor)
                    {
                        dn.ActorName = sourceActor.Name;
                        if (dn.OverridePortrait)
                        {
                            if (dn.Portrait != null)
                            {
                                sourceActor.CurrentPortrait = dn.Portrait;
                            }
                        }
                        else
                        {
                            dn.Portrait = sourceActor.CurrentPortrait;
                        }
                    }
                    else if (dn.UseTargetActor)
                    {
                        dn.ActorName = targetActor.Name;
                        if (dn.OverridePortrait)
                        {
                            if (dn.Portrait != null)
                            {
                                targetActor.CurrentPortrait = dn.Portrait;
                            }
                        }
                        else
                        {
                            dn.Portrait = targetActor.CurrentPortrait;
                        }
                    }

                    actorNameUnderPortraitText.text = dn.ActorName.ParseObject(target: dn);
                    actorNameOverPortraitText.text = dn.ActorName.ParseObject(target: dn);

                    StopTypeWriter();

                    DisplayTextWithTypewriter(text: dn.DialogueText.ParseObject(target: dn), splitIntoChunks: true);

                    // _typeWriter = StartCoroutine(TypewriterText(dn.DialogueText.ParseObject(dn), typewriterTextDelayTime, dialogueText));

                    backgroundPortraitBox.SetActive(value: b.GetPortrait() != null);
                    actorImage.sprite = b.GetPortrait();
                    backgroundActorNameUnderPortraitBox.SetActive(value: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() != null);
                    backgroundActorNameOverDialogueBox.SetActive(value: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() == null);
                    break;
                case nameof(ChoiceDialogueNode):
                    var cdn = b as ChoiceDialogueNode;
                    if (cdn == null)
                    {
                        break;
                    }

                    if (dialogueBoxOc != null)
                    {
                        dialogueBoxOc.SetObjectActive(active: !string.IsNullOrEmpty(value: cdn.DialogueText));
                    }
                    else
                    {
                        dialogueBox.SetActive(value: !string.IsNullOrEmpty(value: cdn.DialogueText));
                    }

                    if (dialogueChoiceContainerOc != null)
                    {
                        dialogueChoiceContainerOc.SetObjectActive(active: true);
                    }
                    else
                    {
                        dialogueChoiceContainer.SetActive(value: true);
                    }

                    if (continueButtonOc != null)
                    {
                        continueButtonOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        continueButton.SetActive(value: false);
                    }

                    if (cdn.UseSourceActor)
                    {
                        cdn.ActorName = sourceActor.Name;
                        if (cdn.OverridePortrait)
                        {
                            sourceActor.CurrentPortrait = cdn.Portrait;
                        }
                        else
                        {
                            cdn.Portrait = sourceActor.CurrentPortrait;
                        }
                    }
                    else if (cdn.UseTargetActor)
                    {
                        cdn.ActorName = targetActor.Name;
                        if (cdn.OverridePortrait)
                        {
                            targetActor.CurrentPortrait = cdn.Portrait;
                        }
                        else
                        {
                            cdn.Portrait = targetActor.CurrentPortrait;
                        }
                    }

                    actorNameUnderPortraitText.text = cdn.ActorName.ParseObject(target: cdn);
                    actorNameOverPortraitText.text = cdn.ActorName.ParseObject(target: cdn);
                    if (backgroundPortraitBoxOc != null)
                    {
                        backgroundPortraitBoxOc.SetObjectActive(active: b.GetPortrait() != null);
                    }
                    else
                    {
                        backgroundPortraitBox.SetActive(value: b.GetPortrait() != null);
                    }

                    actorImage.sprite = b.GetPortrait();

                    if (backgroundActorNameUnderPortraitBoxOc != null)
                    {
                        backgroundActorNameUnderPortraitBoxOc.SetObjectActive(active: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() != null);
                    }
                    else
                    {
                        backgroundActorNameUnderPortraitBox.SetActive(value: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() != null);
                    }

                    if (backgroundActorNameOverDialogueBoxOc != null)
                    {
                        backgroundActorNameOverDialogueBoxOc.SetObjectActive(active: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() == null);
                    }
                    else
                    {
                        backgroundActorNameOverDialogueBox.SetActive(value: !string.IsNullOrWhiteSpace(value: actorNameUnderPortraitText.text) && b.GetPortrait() == null);
                    }

                    StartCoroutine(routine: UpdateDialogueChoices(node: cdn)); //Instantiates the buttons

                    break;
                case nameof(VariableNode):
                    var vn = b as VariableNode;
                    if (vn == null)
                    {
                        break;
                    }

                    Variable<int> intVar = null;
                    Variable<long> longVar = null;
                    Variable<short> shortVar = null;
                    Variable<double> doubleVar = null;
                    Variable<decimal> decimalVar = null;
                    Variable<float> floatVar = null;
                    Variable<bool> boolVar = null;
                    Variable<string> stringVar = null;
                    Variable<ComparableVector2> vector2Var = null;
                    Variable<ComparableVector3> vector3Var = null;
                    Variable<DateTime> dateTimeVar = null;

                    var existingIntVar = vn.Variables.IntVariables[variableName: vn.VariableName];
                    var existingLongVar = vn.Variables.LongVariables[variableName: vn.VariableName];
                    var existingShortVar = vn.Variables.ShortVariables[variableName: vn.VariableName];
                    var existingDoubleVar = vn.Variables.DoubleVariables[variableName: vn.VariableName];
                    var existingDecimalVar = vn.Variables.DecimalVariables[variableName: vn.VariableName];
                    var existingFloatVar = vn.Variables.FloatVariables[variableName: vn.VariableName];
                    var existingBoolVar = vn.Variables.BoolVariables[variableName: vn.VariableName];
                    var existingStringVar = vn.Variables.StringVariables[variableName: vn.VariableName];
                    var existingVector2Var = vn.Variables.Vector2Variables[variableName: vn.VariableName];
                    var existingVector3Var = vn.Variables.Vector3Variables[variableName: vn.VariableName];
                    var existingDateTimeVar = vn.Variables.DateTimeVariables[variableName: vn.VariableName];


                    try
                    {
                        switch (vn.VariableType)
                        {
                            case VariableType.Int:
                                intVar = new Variable<int>(name: vn.VariableName, value: (int)vn.VariableValue);
                                break;
                            case VariableType.Long:
                                longVar = new Variable<long>(name: vn.VariableName, value: (long)vn.VariableValue);
                                break;
                            case VariableType.Short:
                                shortVar = new Variable<short>(name: vn.VariableName, value: (short)vn.VariableValue);
                                break;
                            case VariableType.Double:
                                doubleVar = new Variable<double>(name: vn.VariableName, value: (double)vn.VariableValue);
                                break;
                            case VariableType.Decimal:
                                decimalVar = new Variable<decimal>(name: vn.VariableName, value: (decimal)vn.VariableValue);
                                break;
                            case VariableType.Float:
                                floatVar = new Variable<float>(name: vn.VariableName, value: (float)vn.VariableValue);
                                break;
                            case VariableType.Bool:
                                boolVar = new Variable<bool>(name: vn.VariableName, value: (bool)vn.VariableValue);
                                break;
                            case VariableType.String:
                                stringVar = new Variable<string>(name: vn.VariableName, value: (string)vn.VariableValue);
                                break;
                            case VariableType.Vector2:
                                vector2Var = new Variable<ComparableVector2>(name: vn.VariableName, value: (ComparableVector2)vn.VariableValue);
                                break;
                            case VariableType.Vector3:
                                vector3Var = new Variable<ComparableVector3>(name: vn.VariableName, value: (ComparableVector3)vn.VariableValue);
                                break;
                            case VariableType.DateTime:
                                dateTimeVar = new Variable<DateTime>(name: vn.VariableName, value: (DateTime)vn.VariableValue);
                                break;
                        }

                        switch (vn.OperatorType)
                        {
                            case Operator.Add:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                existingIntVar.Value += intVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: intVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: intVar.Name))
                                                {
                                                    vn.Variables.IntVariables.Variables.Add(item: new Variable<int>(name: intVar.Name, value: intVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                existingLongVar.Value += longVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: longVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: longVar.Name))
                                                {
                                                    vn.Variables.LongVariables.Variables.Add(item: new Variable<long>(name: longVar.Name, value: longVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                existingShortVar.Value += shortVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: shortVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: shortVar.Name))
                                                {
                                                    vn.Variables.ShortVariables.Variables.Add(item: new Variable<short>(name: shortVar.Name, value: shortVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                existingDoubleVar.Value += doubleVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: doubleVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: doubleVar.Name))
                                                {
                                                    vn.Variables.DoubleVariables.Variables.Add(item: new Variable<double>(name: doubleVar.Name, value: doubleVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                existingDecimalVar.Value += decimalVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: decimalVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: decimalVar.Name))
                                                {
                                                    vn.Variables.DecimalVariables.Variables.Add(item: new Variable<decimal>(name: decimalVar.Name, value: decimalVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                existingFloatVar.Value += floatVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: floatVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: floatVar.Name))
                                                {
                                                    vn.Variables.FloatVariables.Variables.Add(item: new Variable<float>(name: floatVar.Name, value: floatVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                existingBoolVar.Value = true;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: boolVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: boolVar.Name))
                                                {
                                                    vn.Variables.BoolVariables.Variables.Add(item: new Variable<bool>(name: boolVar.Name, value: boolVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                existingStringVar.Value += stringVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: stringVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: stringVar.Name))
                                                {
                                                    vn.Variables.StringVariables.Variables.Add(item: new Variable<string>(name: stringVar.Name, value: stringVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                existingVector2Var.Value += vector2Var.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: vector2Var.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: vector2Var.Name))
                                                {
                                                    vn.Variables.Vector2Variables.Variables.Add(item: new Variable<ComparableVector2>(name: vector2Var.Name, value: vector2Var.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                existingVector3Var.Value += vector3Var.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: vector3Var.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: vector3Var.Name))
                                                {
                                                    vn.Variables.Vector3Variables.Variables.Add(item: new Variable<ComparableVector3>(name: vector3Var.Name, value: vector3Var.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                existingDateTimeVar.Value = dateTimeVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: dateTimeVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: dateTimeVar.Name))
                                                {
                                                    vn.Variables.DateTimeVariables.Variables.Add(item: new Variable<DateTime>(name: dateTimeVar.Name, value: dateTimeVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                }

                                NextNode(fieldName: nameof(vn.ExitTrue));
                                break;
                            case Operator.Subtract:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                existingIntVar.Value -= intVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                existingLongVar.Value -= longVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                existingShortVar.Value -= shortVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                existingDoubleVar.Value -= doubleVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                existingDecimalVar.Value -= decimalVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                existingFloatVar.Value -= floatVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                existingBoolVar.Value = false;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                existingStringVar.Value.Remove(startIndex: stringVar.Value.Length);
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                existingVector2Var.Value -= vector2Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                existingVector3Var.Value -= vector3Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                existingDateTimeVar.Value = default;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                NextNode(fieldName: nameof(vn.ExitTrue));
                                break;
                            case Operator.Multiply:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                existingIntVar.Value *= intVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                existingLongVar.Value *= longVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                existingShortVar.Value *= shortVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                existingDoubleVar.Value *= doubleVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                existingDecimalVar.Value *= decimalVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                existingFloatVar.Value *= floatVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                existingBoolVar.Value = true;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        NextNode(fieldName: nameof(vn.ExitFalse));
                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                existingVector2Var.Value *= vector2Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                existingVector3Var.Value *= vector3Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        NextNode(fieldName: nameof(vn.ExitFalse));
                                        break;
                                }

                                NextNode(fieldName: nameof(vn.ExitTrue));
                                break;
                            case Operator.Divide:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                existingIntVar.Value /= intVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                existingLongVar.Value /= longVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                existingShortVar.Value /= shortVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                existingDoubleVar.Value /= doubleVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                existingDecimalVar.Value /= decimalVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                existingFloatVar.Value /= floatVar.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        NextNode(fieldName: nameof(vn.ExitFalse));
                                        break;
                                    case VariableType.String:
                                        NextNode(fieldName: nameof(vn.ExitFalse));
                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                existingVector2Var.Value /= vector2Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                existingVector3Var.Value /= vector3Var.Value;
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        NextNode(fieldName: nameof(vn.ExitFalse));
                                        break;
                                }

                                NextNode(fieldName: nameof(vn.ExitTrue));
                                break;
                            case Operator.Set:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                existingIntVar.Value = intVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: intVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: intVar.Name))
                                                {
                                                    vn.Variables.IntVariables.Variables.Add(item: new Variable<int>(name: intVar.Name, value: intVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                existingLongVar.Value = longVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: longVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: longVar.Name))
                                                {
                                                    vn.Variables.LongVariables.Variables.Add(item: new Variable<long>(name: longVar.Name, value: longVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                existingShortVar.Value = shortVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: shortVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: shortVar.Name))
                                                {
                                                    vn.Variables.ShortVariables.Variables.Add(item: new Variable<short>(name: shortVar.Name, value: shortVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                existingDoubleVar.Value = doubleVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: doubleVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: doubleVar.Name))
                                                {
                                                    vn.Variables.DoubleVariables.Variables.Add(item: new Variable<double>(name: doubleVar.Name, value: doubleVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                existingDecimalVar.Value = decimalVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: decimalVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: decimalVar.Name))
                                                {
                                                    vn.Variables.DecimalVariables.Variables.Add(item: new Variable<decimal>(name: decimalVar.Name, value: decimalVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                existingFloatVar.Value = floatVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: floatVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: floatVar.Name))
                                                {
                                                    vn.Variables.FloatVariables.Variables.Add(item: new Variable<float>(name: floatVar.Name, value: floatVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                existingBoolVar.Value = boolVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: boolVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: boolVar.Name))
                                                {
                                                    vn.Variables.BoolVariables.Variables.Add(item: new Variable<bool>(name: boolVar.Name, value: boolVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                existingStringVar.Value = stringVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: stringVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: stringVar.Name))
                                                {
                                                    vn.Variables.StringVariables.Variables.Add(item: new Variable<string>(name: stringVar.Name, value: stringVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                existingVector2Var.Value = vector2Var.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: vector2Var.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: vector2Var.Name))
                                                {
                                                    vn.Variables.Vector2Variables.Variables.Add(item: new Variable<ComparableVector2>(name: vector2Var.Name, value: vector2Var.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                existingVector3Var.Value = vector3Var.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: vector3Var.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: vector3Var.Name))
                                                {
                                                    vn.Variables.Vector3Variables.Variables.Add(item: new Variable<ComparableVector3>(name: vector3Var.Name, value: vector3Var.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                existingDateTimeVar.Value = dateTimeVar.Value;
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(value: dateTimeVar.Name))
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                                else if (!DoesVariableExist(variableName: dateTimeVar.Name))
                                                {
                                                    vn.Variables.DateTimeVariables.Variables.Add(item: new Variable<DateTime>(name: dateTimeVar.Name, value: dateTimeVar.Value));
                                                }
                                                else
                                                {
                                                    NextNode(fieldName: nameof(vn.ExitFalse));
                                                }
                                            }
                                        }

                                        break;
                                }

                                NextNode(fieldName: nameof(vn.ExitTrue));
                                break;
                            case Operator.EqualTo:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: existingIntVar.Value.Equals(obj: intVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: existingLongVar.Value.Equals(obj: longVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: existingShortVar.Value.Equals(obj: shortVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: existingDoubleVar.Value.Equals(obj: doubleVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: existingDecimalVar.Value.Equals(value: decimalVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: existingFloatVar.Value.Equals(obj: floatVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: existingBoolVar.Value.Equals(obj: boolVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: existingStringVar.Value.Equals(value: stringVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: existingVector2Var.Value.Equals(other: vector2Var.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: existingVector3Var.Value.Equals(other: vector3Var.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: existingDateTimeVar.Value.Equals(value: dateTimeVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                            case Operator.NotEqualTo:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: !existingIntVar.Value.Equals(obj: intVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: !existingLongVar.Value.Equals(obj: longVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: !existingShortVar.Value.Equals(obj: shortVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: !existingDoubleVar.Value.Equals(obj: doubleVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: !existingDecimalVar.Value.Equals(value: decimalVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: !existingFloatVar.Value.Equals(obj: floatVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: !existingBoolVar.Value.Equals(obj: boolVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: !existingStringVar.Value.Equals(value: stringVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: !existingVector2Var.Value.Equals(other: vector2Var.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: !existingVector3Var.Value.Equals(other: vector3Var.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: !existingDateTimeVar.Value.Equals(value: dateTimeVar.Value) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                            case Operator.GreaterThan:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: existingIntVar.GreaterThan(other: intVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: existingLongVar.GreaterThan(other: longVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: existingShortVar.GreaterThan(other: shortVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: existingDoubleVar.GreaterThan(other: doubleVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: existingDecimalVar.GreaterThan(other: decimalVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: existingFloatVar.GreaterThan(other: floatVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: existingBoolVar.GreaterThan(other: boolVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: existingStringVar.GreaterThan(other: stringVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: existingVector2Var.GreaterThan(other: vector2Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: existingVector3Var.GreaterThan(other: vector3Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: existingDateTimeVar.GreaterThan(other: dateTimeVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                            case Operator.GreaterThanOrEqual:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: existingIntVar.GreaterThanOrEqual(other: intVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: existingLongVar.GreaterThanOrEqual(other: longVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: existingShortVar.GreaterThanOrEqual(other: shortVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: existingDoubleVar.GreaterThanOrEqual(other: doubleVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: existingDecimalVar.GreaterThanOrEqual(other: decimalVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: existingFloatVar.GreaterThanOrEqual(other: floatVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: existingBoolVar.GreaterThanOrEqual(other: boolVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: existingStringVar.GreaterThanOrEqual(other: stringVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: existingVector2Var.GreaterThanOrEqual(other: vector2Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: existingVector3Var.GreaterThanOrEqual(other: vector3Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: existingDateTimeVar.GreaterThanOrEqual(other: dateTimeVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                            case Operator.LessThan:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: existingIntVar.LessThan(other: intVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: existingLongVar.LessThan(other: longVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: existingShortVar.LessThan(other: shortVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: existingDoubleVar.LessThan(other: doubleVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: existingDecimalVar.LessThan(other: decimalVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: existingFloatVar.LessThan(other: floatVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: existingBoolVar.LessThan(other: boolVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: existingStringVar.LessThan(other: stringVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: existingVector2Var.LessThan(other: vector2Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: existingVector3Var.LessThan(other: vector3Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: existingDateTimeVar.LessThan(other: dateTimeVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                            case Operator.LessThanOrEqual:
                                switch (vn.VariableType)
                                {
                                    case VariableType.Int:
                                        if (intVar != null)
                                        {
                                            if (existingIntVar != null)
                                            {
                                                NextNode(fieldName: existingIntVar.LessThanOrEqual(other: intVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Long:
                                        if (longVar != null)
                                        {
                                            if (existingLongVar != null)
                                            {
                                                NextNode(fieldName: existingLongVar.LessThanOrEqual(other: longVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Short:
                                        if (shortVar != null)
                                        {
                                            if (existingShortVar != null)
                                            {
                                                NextNode(fieldName: existingShortVar.LessThanOrEqual(other: shortVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Double:
                                        if (doubleVar != null)
                                        {
                                            if (existingDoubleVar != null)
                                            {
                                                NextNode(fieldName: existingDoubleVar.LessThanOrEqual(other: doubleVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Decimal:
                                        if (decimalVar != null)
                                        {
                                            if (existingDecimalVar != null)
                                            {
                                                NextNode(fieldName: existingDecimalVar.LessThanOrEqual(other: decimalVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Float:
                                        if (floatVar != null)
                                        {
                                            if (existingFloatVar != null)
                                            {
                                                NextNode(fieldName: existingFloatVar.LessThanOrEqual(other: floatVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Bool:
                                        if (boolVar != null)
                                        {
                                            if (existingBoolVar != null)
                                            {
                                                NextNode(fieldName: existingBoolVar.LessThanOrEqual(other: boolVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.String:
                                        if (stringVar != null)
                                        {
                                            if (existingStringVar != null)
                                            {
                                                NextNode(fieldName: existingStringVar.LessThanOrEqual(other: stringVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector2:
                                        if (vector2Var != null)
                                        {
                                            if (existingVector2Var != null)
                                            {
                                                NextNode(fieldName: existingVector2Var.LessThanOrEqual(other: vector2Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.Vector3:
                                        if (vector3Var != null)
                                        {
                                            if (existingVector3Var != null)
                                            {
                                                NextNode(fieldName: existingVector3Var.LessThanOrEqual(other: vector3Var) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                    case VariableType.DateTime:
                                        if (dateTimeVar != null)
                                        {
                                            if (existingDateTimeVar != null)
                                            {
                                                NextNode(fieldName: existingDateTimeVar.LessThanOrEqual(other: dateTimeVar) ? nameof(vn.ExitTrue) : nameof(vn.ExitFalse));
                                            }
                                            else
                                            {
                                                NextNode(fieldName: nameof(vn.ExitFalse));
                                            }
                                        }

                                        break;
                                }

                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(message: $"Error: {e.Message}");
                    }

                    break;

                case "ReferenceStateNode":
                    var rsn = b as ReferenceStateNode;
                    if (rsn == null)
                    {
                        break;
                    }

                    var sourceID = rsn.OverrideSourceID ? rsn.OverrideSourceIDValue : sourceActor.ID;
                    var targetID = rsn.OverrideTargetID ? rsn.OverrideTargetIDValue : targetActor.ID;
                    MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Dialogue,
                        message: new DialogueMessage(state: DialogueState.SetReferenceState, referenceState: rsn.ReferenceState, graph: selectedGraph, interaction: null, sourceID: sourceID,
                            targetID: targetID));
                    NextNode(fieldName: nameof(rsn.ExitTrue));
                    break;

                case "AudioNode":
                    var an = b as AudioNode;
                    if (an == null)
                    {
                        break;
                    }

                    switch (an.AudioChannel)
                    {
                        case AudioChannel.Master:
                            switch (an.AudioOperation)
                            {
                                case AudioOperation.Play:
                                case AudioOperation.Resume:
                                case AudioOperation.Pause:
                                case AudioOperation.Stop:
                                    NextNode(fieldName: nameof(an.ExitFalse));
                                    break;
                                case AudioOperation.SetVolume:
                                    MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Audio,
                                        message: new AudioMessage(audioObject: an.Audio, operation: an.AudioOperation, audioChannel: an.AudioChannel, useRandomVolume: an.UseRandomVolume,
                                            useRandomPitch: an.UseRandomPitch, volume: an.AudioVolume, pitch: an.AudioPitch));
                                    NextNode(fieldName: nameof(an.ExitTrue));
                                    break;
                                default:
                                    NextNode(fieldName: nameof(an.ExitFalse));
                                    break;
                            }

                            break;
                        case AudioChannel.Music:
                            if (AudioManager.Instance.Music.Exists(match: x => x.Equals(other: an.Audio)))
                            {
                                MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Audio,
                                    message: new AudioMessage(audioObject: an.Audio, operation: an.AudioOperation, audioChannel: an.AudioChannel, useRandomVolume: an.UseRandomVolume,
                                        useRandomPitch: an.UseRandomPitch, volume: an.AudioVolume, pitch: an.AudioPitch));
                                NextNode(fieldName: nameof(an.ExitTrue));
                            }
                            else
                            {
                                NextNode(fieldName: nameof(an.ExitFalse));
                            }

                            break;
                        case AudioChannel.SoundEffects:
                            if (AudioManager.Instance.SoundEffects.Exists(match: x => x.Equals(other: an.Audio)))
                            {
                                MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Audio,
                                    message: new AudioMessage(audioObject: an.Audio, operation: an.AudioOperation, audioChannel: an.AudioChannel, useRandomVolume: an.UseRandomVolume,
                                        useRandomPitch: an.UseRandomPitch, volume: an.AudioVolume, pitch: an.AudioPitch));
                                NextNode(fieldName: nameof(an.ExitTrue));
                            }
                            else
                            {
                                NextNode(fieldName: nameof(an.ExitFalse));
                            }

                            break;
                    }

                    break;
                case "MessageSenderNode":
                    var msn = b as MessageSenderNode;
                    if (msn == null)
                    {
                        break;
                    }

                    switch (msn.SendAction)
                    {
                        case MessageSendAction.SendImmediate:
                            msn.MessageConfig.SendImmediate(channel: msn.Channel);
                            break;
                        case MessageSendAction.QueueMessage:
                            msn.MessageConfig.Send(channel: msn.Channel);
                            break;
                        case MessageSendAction.Broadcast:
                            msn.MessageConfig.Broadcast();
                            break;
                    }

                    NextNode(fieldName: nameof(msn.Exit));
                    break;
                case "ExitNode_NoLoop_toStart":
                    Debug.LogWarning(message: "Dialogue Ended");
                    if (dialogueBoxOc != null)
                    {
                        dialogueBoxOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        dialogueBox.SetActive(value: false);
                    }

                    if (continueButtonOc != null)
                    {
                        continueButtonOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        continueButton.SetActive(value: false);
                    }

                    //TODO: Remove possibly? If this is not in here controllers activate the dialogue again on a single press
                    yield return new WaitForSeconds(seconds: 0.05f);
                    // MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TouchControlMessage(true));

                    MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Dialogue,
                        message: new DialogueMessage(state: DialogueState.End, referenceState: string.Empty, graph: selectedGraph, interaction: null, sourceID: b.SourceActor?.ID,
                            targetID: b.TargetActor?.ID));
                    // MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
                    _isDialogueOpen = false;
                    break;
                case "ExitNode":
                    if (dialogueBoxOc != null)
                    {
                        dialogueBoxOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        dialogueBox.SetActive(value: false);
                    }

                    if (continueButtonOc != null)
                    {
                        continueButtonOc.SetObjectActive(active: false);
                    }
                    else
                    {
                        continueButton.SetActive(value: false);
                    }

                    selectedGraph.Start();
                    //TODO: Remove possibly? If this is not in here controllers activate the dialogue again on a single press
                    yield return new WaitForSeconds(seconds: 0.05f);
                    // MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TouchControlMessage(true));
                    MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.Dialogue,
                        message: new DialogueMessage(state: DialogueState.End, referenceState: string.Empty, graph: selectedGraph, interaction: null, sourceID: b.SourceActor?.ID,
                            targetID: b.TargetActor?.ID));
                    // MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, false));
                    _isDialogueOpen = false;
                    break;
            }
        }

        public virtual void SetSelectedGameObject(GameObject go)
        {
            // _eventSystem.SetSelectedGameObject(null);
            // _eventSystem.SetSelectedGameObject(go);
            EventSystem.current.SetSelectedGameObject(selected: null);
            EventSystem.current.SetSelectedGameObject(selected: go);
        }

        public virtual bool DoesVariableExist(string variableName)
        {
            return selectedGraph.current.Variables.IntVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.LongVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.ShortVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DoubleVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DecimalVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.FloatVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.BoolVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.StringVariables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.Vector2Variables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.Vector3Variables.Variables.Any(predicate: v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DateTimeVariables.Variables.Any(predicate: v => v.Name == variableName);
        }

        public virtual void DialogueMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<DialogueMessage>().HasValue)
            {
                return;
            }

            var data = message.Message<DialogueMessage>().GetValueOrDefault();
            switch (data.State)
            {
                case DialogueState.SetReferenceState:
                    break;
                case DialogueState.SetGraph:
                    break;
                case DialogueState.SetInteraction:
                    break;
                case DialogueState.Start:
                    // MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));

                    if (data.Graph != null)
                    {
                        StartDialogue(graph: data.Graph);
                        break;
                    }

                    if (data.Interaction != null)
                    {
                        StartDialogue(graph: data.Interaction.Graph);
                        break;
                    }

                    //TODO: Remove dependacy on the DialogueManager
                    var matchingInteractions = DialogueManager.Instance.CurrentDialogueManagerObject.GetDialogueInteractions(includeCompleted: false);
                    var interaction = matchingInteractions.FirstOrDefault(predicate: x => x.ReferenceState.Equals(value: data.ReferenceState));
                    if (interaction != null)
                    {
                        StartDialogue(graph: interaction.Graph);
                    }

                    break;
                case DialogueState.End:
                    MessageSystem.MessageManager.SendImmediate(channel: MessageChannels.GameFlow, message: new PauseMessage(source: this, isPaused: false));
                    break;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnInactive();
        }
    }
}