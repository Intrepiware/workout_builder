import { ChangeEvent, useState } from "react";

interface Part {
  id: number;
  name: string;
  isMuscle: boolean;
}

interface FocusAndActivationGroupData {
  focusPartId: number;
  activationIds: number[];
}

export function FocusAndActivationGroup(props: any) {
  const [focusId, activationIds, parts] = parseProps(props);
  const partOptions = parts.sort((a, b) => a.name.localeCompare(b.name));
  const focusOptions = partOptions.filter((x) => x.isMuscle);
  const [data, setData] = useState<FocusAndActivationGroupData>({
    focusPartId: focusId,
    activationIds: activationIds,
  });

  function handleFocusChange(event: ChangeEvent<HTMLSelectElement>): void {
    const focusPartId = parseFloat(event.target.value || "-1");
    const activationIds = data.activationIds.filter((x) => x != focusPartId);
    setData({ focusPartId, activationIds });
  }

  function handleActivationChange(event: ChangeEvent<HTMLSelectElement>): void {
    const activationIds = Array.from(event.target.selectedOptions).map((x) =>
      parseFloat(x.value)
    );
    setData((x) => ({ ...x, activationIds }));
  }

  return (
    <>
      <div className="field">
        <label className="label">Muscle Focus</label>
        <div className="control">
          <div className="select">
            <select
              value={data.focusPartId}
              name="focusPartId"
              onChange={handleFocusChange}
            >
              <option></option>
              {focusOptions.map((x) => (
                <option value={x.id} key={x.id}>
                  {x.name}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>
      <div className="field">
        <label className="label">Activation Parts</label>
        <div className="control">
          <div className="select is-multiple">
            <select
              multiple
              size={8}
              name="activationParts"
              onChange={handleActivationChange}
              value={data.activationIds.map((x) => x.toString())}
            >
              {partOptions
                .filter((x) => x.id != data.focusPartId)
                .map((x) => (
                  <option value={x.id} key={x.id}>
                    {x.name}
                  </option>
                ))}
            </select>
          </div>
        </div>
      </div>
    </>
  );
}

const parseProps: (props: any) => [number, number[], Part[]] = (props) => {
  const parts: Part[] = props.parts.split(";").map((x: string) => {
    const parts = x.split(":");
    return {
      id: parseFloat(parts[0]),
      name: parts[1],
      isMuscle: parts[2] === "True",
    };
  });

  const focusId = parseInt(props.focusId || 0, 10);
  const activationids: number[] =
    props.activationIds?.split(",").map((x: string) => parseInt(x, 10)) || [];
  return [focusId, activationids, parts];
};
