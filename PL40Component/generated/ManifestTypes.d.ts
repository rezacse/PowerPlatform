/*
*This is auto generated from the ControlManifest.Input.xml file
*/

// Define IInputs and IOutputs Type. They should match with ControlManifest.
export interface IInputs {
    textValue: ComponentFramework.PropertyTypes.StringProperty;
    isUpperCaseOnly: ComponentFramework.PropertyTypes.TwoOptionsProperty;
}
export interface IOutputs {
    textValue?: string;
    isUpperCaseOnly?: boolean;
}
