import axios from "axios";
import AuthService from "./AuthService";

const eventApi = 'https://localhost:7280';

class EventService {
  async getPublished()
  {
    const response = await axios.get(eventApi + '/events/published');
    return response.data;
  }

  async getNotPublished()
  {
    const response = await axios.get(eventApi + '/events/notPublished', {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
    return response.data;
  }

  async create(event)
  {
    await axios.post(eventApi + '/events', event, {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
  }
}
export default new EventService();