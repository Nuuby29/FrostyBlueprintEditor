﻿using System.Collections.ObjectModel;
using BlueprintEditorPlugin.Models.Connections;
using BlueprintEditorPlugin.Utils;
using Frosty.Core.Controls;
using FrostySdk.Ebx;

namespace BlueprintEditorPlugin.Models.Types.NodeTypes.Entity.ExampleTypes
{
    /// <summary>
    /// This is a more advanced demonstration, for a simple demonstration <see cref="CompareBoolEntityData"/>
    /// This demonstrates creating events and properties based off of the property grid
    /// </summary>
    public class SelectEventEntityData : EntityNode
    {
        /// <summary>
        /// This is the name that will be displayed in the editor.
        /// This can be set to whatever you want, and can also be modified via code.
        /// </summary>
        public override string Name { get; set; } = "Select Event";
        
        /// <summary>
        /// This is the name of the type this applies to.
        /// This HAS to be the exact name of the type, so in this case, CompareBoolEntityData
        /// This value is static.
        /// </summary>
        public override string ObjectType { get; set; } = "SelectEventEntityData";

        /// <summary>
        /// These are all of the inputs this has.
        /// Each input allows you to customize the Title, so its name
        /// And its type, so Event, Property, and Link
        /// </summary>
        public override ObservableCollection<InputViewModel> Inputs { get; set; } =
            new ObservableCollection<InputViewModel>()
            {
                new InputViewModel() {Title = "In", Type = ConnectionType.Event},
                new InputViewModel() {Title = "Selection", Type = ConnectionType.Property}
            };

        /// <summary>
        /// These are all of the outputs this has.
        /// Each input allows you to customize the Title, so its name
        /// And its type, so Event, Property, and Link
        /// </summary>
        public override ObservableCollection<OutputViewModel> Outputs { get; set; } =
            new ObservableCollection<OutputViewModel>();

        /// <summary>
        /// Don't use an initializer when working with these, instead, override the OnCreation method.
        /// This triggers when the node gets created
        /// that way you can do things like change the Name based on one of its inputs, or one of the objects properties
        /// </summary>
        public override void OnCreation()
        {
            base.OnCreation();
            foreach (CString eventName in Object.Events) //Go through all of the Events this SelectEvent has
            {
                //And for each one, add it to our Outputs
                Outputs.Add(new OutputViewModel() {Title = eventName.ToString(), Type = ConnectionType.Event});
                Inputs.Add(new InputViewModel() {Title = $"Select{eventName.ToString()}", Type = ConnectionType.Event});
            }
            
            foreach (InputViewModel input in Inputs)
            {
                NodeUtils.PortRealmFromObject(Object, input);
            }

            foreach (OutputViewModel output in Outputs)
            {
                NodeUtils.PortRealmFromObject(Object, output);
            }
        }

        /// <summary>
        /// This will trigger whenever the SelectEvent is modified
        /// Since we want to make sure our SelectEvent is in sync with the property grid, we redo our OnCreation
        /// </summary>
        public override void OnModified(ItemModifiedEventArgs args)
        {
            //TODO: Fix this. This will remove all of the connections(since we are removing the inputs/outputs) and cause bugs
            Outputs.Clear();
            Inputs.Clear();
            foreach (CString eventName in Object.Events) //Go through all of the Events this SelectEvent has
            {
                //And for each one, add it to our Outputs
                Outputs.Add(new OutputViewModel() {Title = eventName.ToString(), Type = ConnectionType.Event});
                Inputs.Add(new InputViewModel() {Title = $"Select{eventName.ToString()}", Type = ConnectionType.Event});
            }
            
            //We want to make sure our Inputs and Outputs are the same realm as us, that way our flags compute properly
            foreach (InputViewModel input in Inputs)
            {
                NodeUtils.PortRealmFromObject(Object, input);
            }

            foreach (OutputViewModel output in Outputs)
            {
                NodeUtils.PortRealmFromObject(Object, output);
            }
        }
    }
}