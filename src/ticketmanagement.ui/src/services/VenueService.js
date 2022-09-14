import axios from "axios";
import AuthService from "./AuthService";

const venueApi = 'https://localhost:7162';

class VenueService {
  async getAll()
  {
    const response = await axios.get(venueApi + '/venues');
    return response.data;
  }

  async getById(id)
  {
    const response = await axios.get(venueApi + `/venues/${id}`, {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
    return response.data;
  }

  async getLayoutsByVenueId(venueId)
  {
    const response = await axios.get(venueApi + `/venues/${venueId}/layouts`, {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
    return response.data;
  }

  async create(venue)
  {
    await axios.post(venueApi + '/venues', venue, {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
  }
}
export default new VenueService();