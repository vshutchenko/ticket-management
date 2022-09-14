import axios from "axios";
import AuthService from "./AuthService";

const venueApi = process.env.REACT_APP_VENUE_API;

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
          'Authorization': `Bearer ${AuthService.getToken()}`
        }
    });
    return response.data;
  }

  async getLayoutsByVenueId(venueId)
  {
    const response = await axios.get(venueApi + `/venues/${venueId}/layouts`, {
        headers: {
          'Authorization': `Bearer ${AuthService.getToken()}`
        }
    });
    return response.data;
  }

  async create(venue)
  {
    await axios.post(venueApi + '/venues', venue, {
        headers: {
          'Authorization': `Bearer ${AuthService.getToken()}`
        }
    });
  }
}
export default new VenueService();