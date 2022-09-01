import React, {Component} from "react";

export class Events extends Component
{
    constructor(props) {
        super(props);

        this.state = {
            events : [],
            loading: true
        }
    }

    componentDidMount() {
        this.populateEventsData();
    }

    async populateEventsData() {
        const response = await fetch('api/events');
        const data = await response.json();
        this.setState({events: data, loading:false});
    }

    renderAllEventsTable(events) {
        return (
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Start date</th>
                        <th>End date</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        events.map(event => (
                        <tr key={event.id}>
                            <td>{event.name}</td>
                            <td>{event.description}</td>
                            <td>{event.startDate}</td>
                            <td>{event.endDate}</td>
                            <td>-</td>
                        </tr>
                        ))
                    }
                </tbody>
            </table>
        )
    }

    render() {

        let content = this.state.loading ? (
            <p>
                <em>Loading...</em>
            </p>
        ) : (
            this.renderAllEventsTable(this.state.events)
        )
        
        return (
            <div>
                <h1>All events</h1>
                <p>Here you can see all events.</p>
                {content}
            </div>
        )
    }
}