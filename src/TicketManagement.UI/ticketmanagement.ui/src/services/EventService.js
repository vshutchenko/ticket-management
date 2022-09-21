import axios from "axios";
import AuthService from "./AuthService";

const eventApi = process.env.REACT_APP_EVENT_API;

class EventService {
  async getPublished() {
    const response = await axios.get(eventApi + '/events/published');
    return response.data;
  }

  async getNotPublished() {
    const response = await axios.get(eventApi + '/events/notPublished', {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
    return response.data;
  }

  async getById(id) {
    const response = await axios.get(eventApi + `/events/${id}`, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
    return response.data;
  }

  async getAreasById(id) {
    const response = await axios.get(eventApi + `/events/${id}/areas`, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
    return response.data;
  }

  async create(event) {
    await axios.post(eventApi + '/events', event, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
  }

  async update(event) {
    await axios.put(eventApi + '/events', event, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
  }

  async delete(id) {
    await axios.delete(eventApi + `/events/${id}`, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
  }
}
export default new EventService();