import { useEffect, useState } from "react";

interface AdvancedOptionsDialogProps {
  onSelectedChange: (data: string[]) => void;
  onClose: () => void;
  allEquipment: string[];
  selectedEquipment: string[];
  visible: boolean;
}

export function AdvancedOptionsDialog({
  onSelectedChange,
  onClose,
  allEquipment,
  selectedEquipment,
  visible,
}: AdvancedOptionsDialogProps) {
  const [equipmentPreset, setEquipmentPreset] = useState("All");

  useEffect(() => {
    const setSelectedEquipment = (data: string[]) => {
      onSelectedChange(data);
    };
    switch (equipmentPreset) {
      case "None":
        setSelectedEquipment([]);
        break;
      case "Bootcamp":
        setSelectedEquipment([
          "Bodyweight",
          "Cones",
          "Space to Run",
          "Dumbbells",
        ]);
        break;
      case "Calisthenics":
        setSelectedEquipment([
          "Bodyweight",
          "Dip Bars",
          "Plyo Box",
          "Pull Up Bar",
          "Step Riser",
          "TRX",
        ]);
        break;
      case "Just Weights":
        setSelectedEquipment(["Dumbbells", "Barbell"]);
        break;
      case "Odd Object Training":
        setSelectedEquipment([
          "Kettlebell",
          "Med Ball",
          "Sandbell",
          "Sandbag",
          "Slamball",
          "Sledge Hammer",
          "Tire",
          "Small Plates",
          "Large Plates",
        ]);
        break;
      case "All":
      default:
        setSelectedEquipment(allEquipment);
        break;
    }
  }, [equipmentPreset]);
  const handleEquipmentToggle = (label: string): void => {
    const selected: string[] = [...selectedEquipment];
    if (selected.includes(label)) {
      selected.splice(selected.indexOf(label), 1);
    } else {
      selected.push(label);
    }
    // setUiElements((x) => ({ ...x, selectedEquipment: selected }));
    onSelectedChange(selected);
  };

  return (
    <div
      className={`modal ${visible ? "is-active" : ""}`}
      id="advanced-options"
    >
      <div className="modal-background"></div>
      <div className="modal-content">
        <div className="box">
          <h3 className="title is-4 is-spaced">Advanced Options</h3>
          <label className="label">Randomization</label>
          <div className="field has-addons">
            <p className="control">
              <button className="button" disabled>
                <span className="icon is-small">
                  <span className="material-symbols-outlined">exercise</span>
                </span>
                <span>By Equipment</span>
              </button>
            </p>
            <p className="control">
              <button className="button is-primary is-selected">
                <span className="material-symbols-outlined"> sprint </span>
                <span>&nbsp;By Exercise</span>
              </button>
            </p>
          </div>
          <div className="field">
            <label className="label">Equipment Presets</label>
            <div className="control">
              <div className="select">
                <select
                  value={equipmentPreset}
                  onChange={(e) => setEquipmentPreset(e.target.value)}
                >
                  <option>All</option>
                  <option>None</option>
                  <option>Bootcamp</option>
                  <option>Calisthenics</option>
                  <option>Just Weights</option>
                  <option>Odd Object Training</option>
                </select>
              </div>
            </div>
          </div>
          <label className="label">Available Equipment</label>
          <div className="buttons">
            <EquipmentButton
              onClick={handleEquipmentToggle}
              label="Bodyweight"
              selectedEquipment={selectedEquipment}
            ></EquipmentButton>
            <EquipmentButton
              onClick={handleEquipmentToggle}
              label="Dumbbells"
              selectedEquipment={selectedEquipment}
            ></EquipmentButton>
            <EquipmentButton
              onClick={handleEquipmentToggle}
              label="Space to Run"
              selectedEquipment={selectedEquipment}
            ></EquipmentButton>
            {allEquipment
              .filter(
                (x) => !["Bodyweight", "Space to Run", "Dumbbells"].includes(x)
              )
              .map((x) => (
                <EquipmentButton
                  key={x}
                  onClick={handleEquipmentToggle}
                  label={x}
                  selectedEquipment={selectedEquipment}
                ></EquipmentButton>
              ))}
          </div>
        </div>
      </div>
      <button
        className="modal-close is-large"
        aria-label="close"
        onClick={onClose}
      ></button>
    </div>
  );
}

function EquipmentButton({
  label,
  selectedEquipment,
  onClick,
}: {
  label: string;
  selectedEquipment: string[];
  onClick: (label: string) => void;
}) {
  return (
    <button
      className={`button ${
        selectedEquipment.includes(label) ? "is-primary" : ""
      }`}
      onClick={() => onClick(label)}
    >
      {label}
    </button>
  );
}
