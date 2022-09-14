import axios from "axios";
import AuthService from "./AuthService";

const eventApi = 'https://localhost:7280';

class EventService {
  async getPublished() {
    const response = await axios.get(eventApi + '/events/published');
    return response.data;
  }

  async getNotPublished() {
    const response = await axios.get(eventApi + '/events/notPublished', {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
    return response.data;
  }

  async getById(id) {
    const response = await axios.get(eventApi + `/events/${id}`, {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
    return response.data;
  }

  async getAreasById(id) {
    const response = await axios.get(eventApi + `/events/${id}/areas`, {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
    return response.data;
  }

  async create(event) {
    await axios.post(eventApi + '/events', event, {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
  }

  async update(event) {
    await axios.put(eventApi + '/events', event, {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
  }

  async delete(id) {
    await axios.delete(eventApi + `/events/${id}`, {
      headers: {
        'Authorization': AuthService.getTokenHeader()
      }
    });
  }
}
export default new EventService();