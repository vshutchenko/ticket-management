import axios from "axios";
import AuthService from "./AuthService";

const venueApi = process.env.REACT_APP_VENUE_API;

class LayoutService {
  async getById(id)
  {
    const response = await axios.get(venueApi + `/layouts/${id}`, {
        headers: {
          'Authorization': `Bearer ${AuthService.getToken()}`
        }
    });
    return response.data;
  }
}
export default new LayoutService();