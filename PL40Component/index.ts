import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class PL40Component implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private myNotifyChnaged: () => void;
    private myMainDiv: HTMLDivElement;
    private myTextBox: HTMLTextAreaElement;
    private mytTextboxHandler: EventListener;

    private myUpercaseOnly: boolean;
    private myLabel: HTMLLabelElement;

    private myButton: HTMLButtonElement;
    private myButtonHandler: EventListener;

    constructor() {
        // Empty
    }

    /**
     * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
     * Data-set values are not initialized here, use updateView.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
     * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
     * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
     * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
     */
    public init(
        context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void,
        state: ComponentFramework.Dictionary,
        container: HTMLDivElement
    ): void {
        // Add control initialization code
        this.myNotifyChnaged = notifyOutputChanged;
        this.myMainDiv = document.createElement('div');

        this.myTextBox = document.createElement('textarea');
        this.myTextBox.value = context.parameters.textValue.raw || '';
        this.myMainDiv.appendChild(this.myTextBox);
        this.mytTextboxHandler = this.myTextboxChanged.bind(this);
        this.myMainDiv.addEventListener("input", this.mytTextboxHandler);

        this.myLabel = document.createElement('label');
        this.myMainDiv.appendChild(this.myLabel);
        this.myUpercaseOnly = context.parameters.isUpperCaseOnly.raw || false;

        this.myButton = document.createElement('button');
        this.myButton.textContent = "Toggle Text";
        this.myButtonHandler = this.myButtonClicked.bind(this);
        this.myButton.addEventListener("click", this.myButtonHandler);
        this.myMainDiv.appendChild(this.myButton);

        container.appendChild(this.myMainDiv)
    }

    public myTextboxChanged() {
        this.myNotifyChnaged();
    }

    public myButtonClicked() {
        this.myUpercaseOnly = !this.myUpercaseOnly;
        this.myNotifyChnaged();
    }

    /**
     * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
     */
    public updateView(context: ComponentFramework.Context<IInputs>): void {
        this.myTextBox.value = context.parameters.textValue.raw || '';

        this.myUpercaseOnly = context.parameters.isUpperCaseOnly.raw || false;

        if (this.myUpercaseOnly) {
            this.myLabel.innerHTML = "UPPER CASE ONLY";
            this.myTextBox.value = this.myTextBox.value.toUpperCase();
        } else {
            this.myLabel.innerHTML = "UPPER/lower CASE";
        }

        this.myNotifyChnaged();

        // Add code to update control view
    }

    /**
     * It is called by the framework prior to a control receiving new data.
     * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
     */
    public getOutputs(): IOutputs {
        return {
            textValue: this.myTextBox.value,
            isUpperCaseOnly: this.myUpercaseOnly
        };
    }

    /**
     * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
     * i.e. cancelling any pending remote calls, removing listeners, etc.
     */
    public destroy(): void {
        // Add code to cleanup control if necessary
    }
}
