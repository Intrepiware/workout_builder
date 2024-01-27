interface PartButtonProps {
  label: string;
  value: number;
  state: "enabled" | "disabled" | "selected";
  name: string;
  onClick: (id: number) => void;
}

export function PartButton({
  name,
  label,
  value,
  state,
  onClick,
}: PartButtonProps) {
  const handleClick = (
    event: React.MouseEvent<HTMLButtonElement, MouseEvent>
  ) => {
    event.preventDefault();
    onClick(value);
  };

  if (state === "disabled") {
    return (
      <button className="button is-disabled" onClick={handleClick} disabled>
        {label}
      </button>
    );
  } else if (state === "selected") {
    return (
      <>
        <button className="button is-primary" onClick={handleClick}>
          {label}
        </button>
        <input type="hidden" name={name} value={value}></input>
      </>
    );
  } else {
    return (
      <button className="button" onClick={handleClick}>
        {label}
      </button>
    );
  }
}
