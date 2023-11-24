import { Component, KeyboardEvent } from "react";

interface Props {
  value: string;
  placeholder: string;
  data: string[];
  controlClassName: string;
  onSelect: (item: string) => void;
  label?: string;
  name?: string;
}

interface State {
  activeIndex: number;
  matches: string[];
  query: string;
  selected: boolean;
}

class Autocomplete extends Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      activeIndex: 0,
      matches: [],
      query: props.value || "",
      selected: !!props.value,
    };

    this.handleKeyPress = this.handleKeyPress.bind(this);
    this.handleSelection = this.handleSelection.bind(this);
    this.updateQuery = this.updateQuery.bind(this);
  }

  handleKeyPress(event: KeyboardEvent<HTMLInputElement>) {
    const { activeIndex, matches, query } = this.state;

    switch (event.which) {
      case 13: // Enter key
        if (matches.length) {
          this.setState({
            activeIndex: 0,
            matches: [],
            query: matches[activeIndex],
            selected: true,
          });
          this.props.onSelect(matches[activeIndex]);
        }
        break;
      case 38: // Up arrow
        this.setState({
          activeIndex: activeIndex >= 1 ? activeIndex - 1 : 0,
        });
        break;
      case 40: // Down arrow
        if (!query && !matches.length)
          this.setState({
            matches: this.props.data,
            activeIndex: 0,
          });
        else
          this.setState({
            activeIndex:
              activeIndex < matches.length - 1
                ? activeIndex + 1
                : matches.length - 1,
          });
        break;
      default:
        break;
    }
  }

  handleSelection(
    event: React.MouseEvent<HTMLAnchorElement, MouseEvent>,
    selection: string
  ) {
    debugger;
    console.log(typeof event);
    event.preventDefault();

    this.setState({
      activeIndex: 0,
      query: selection,
      matches: [],
      selected: true,
    });
    this.props.onSelect(selection);
  }

  updateQuery(e: React.ChangeEvent<HTMLInputElement>) {
    const { data } = this.props;

    if (!this.state.selected) {
      const query = e.target.value;
      this.setState({
        matches:
          query.length >= 2
            ? data.filter(
                (item) => item.toUpperCase().indexOf(query.toUpperCase()) >= 0
              )
            : [],
        query,
      });
    } else {
      if ((e.nativeEvent as InputEvent).inputType === "deleteContentBackward") {
        this.setState({
          matches: [],
          query: "",
          selected: false,
        });
      }
    }
  }

  render() {
    const { label, name, placeholder, controlClassName } = this.props;
    const { activeIndex, matches, query } = this.state;

    return (
      <div className="field">
        {label && <label className="label">{label}</label>}
        <div className={`control ${controlClassName || ""}`}>
          <div className={`dropdown ${matches.length > 0 ? "is-active" : ""}`}>
            <div className="dropdown-trigger">
              <input
                type="text"
                className="input"
                name={name}
                value={query}
                onChange={this.updateQuery}
                onKeyDown={this.handleKeyPress}
                placeholder={placeholder}
              />
            </div>
            <div className="dropdown-menu">
              {matches.length > 0 && (
                <div className="dropdown-content">
                  {matches.map((match, index) => (
                    <a
                      className={`dropdown-item ${
                        index === activeIndex ? "is-active" : ""
                      }`}
                      href="/"
                      key={match}
                      onClick={(event) => this.handleSelection(event, match)}
                    >
                      {match}
                    </a>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Autocomplete;
