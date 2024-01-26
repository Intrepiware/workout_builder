interface Part {
  id: number;
  name: string;
  isMuscle: boolean;
}

export function FocusAndActivationGroup(props: any) {
  const [focusId, activationIds, parts] = parseProps(props);
  const partOptions = parts.sort((a, b) => a.name.localeCompare(b.name));
  const focusOptions = partOptions.filter((x) => x.isMuscle);
  return (
    <>
      <label>Muscle Focus</label>
      <select value={focusId}>
        {focusOptions.map((x) => (
          <option value={x.id}>{x.name}</option>
        ))}
      </select>
      <label>Activated Parts</label>
      <select multiple>
        {partOptions.map((x) => (
          <option value={x.id} selected={activationIds.some((y) => y === x.id)}>
            {x.name}
          </option>
        ))}
      </select>
    </>
  );
}

const parseProps: (props: any) => [number, number[], Part[]] = (props) => {
  const parts: Part[] = props.parts.split(";").map((x: string) => {
    const parts = x.split(":");
    return {
      id: parseFloat(parts[0]),
      name: parts[1],
      isMuscle: parts[2] === "true",
    };
  });

  const focusId = parseInt(props.focusId, 10);
  const activationids: number[] =
    props.activationIds?.split(",").map((x: string) => parseInt(x, 10)) || [];
  return [focusId, activationids, parts];
};
