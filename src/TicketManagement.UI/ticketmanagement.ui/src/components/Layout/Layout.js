import { useState, React } from "react";
import { useNavigate } from "react-router";
import { Link } from "react-router-dom";
import AuthService from '../../services/AuthService';
import { useTranslation } from 'react-i18next';
import Cookies from 'universal-cookie';

import {
  Collapse,
  Navbar,
  NavbarToggler,
  NavbarBrand,
  Nav,
  NavItem,
  NavLink,
  UncontrolledDropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem
} from 'reactstrap';

const mvcAppUrl = process.env.REACT_APP_MVC_APP;

export default function Layout() {
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [isOpen, setIsOpen] = useState(false);

  const cookies = new Cookies();
  if (AuthService.isAuthenticated()) {
    cookies.set('jwt', AuthService.getToken(), { path: '/' });
  }

  function logout() {
    AuthService.logout();
    cookies.remove('jwt');
    navigate("/");
  }

  return (

    <div>
      <Navbar color="light" expand="md" className="fs-5 py-3">
        <NavbarBrand className="me-5 fs-4" href="/">Ticket management</NavbarBrand>
        <NavbarToggler onClick={() => setIsOpen(!isOpen)} />
        <Collapse isOpen={isOpen} navbar>
          <Nav navbar className="container-fluid">
            <NavItem>
              <NavLink href="/">{t("Events")}</NavLink>
            </NavItem>
            {AuthService.isEventManager() && (
              <>
                <NavItem>
                  <NavLink href="/Event/NotPublishedEvents">{t("Not published events")}</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink href={`${mvcAppUrl}/EventImport/ImportEvents`}>{t("Event import")}</NavLink>
                </NavItem>
              </>)
            }
            {AuthService.isVenueManager() && (
              <NavItem>
                <NavLink href={`${mvcAppUrl}/Venue/VenueList`}>{t("Venue management")}</NavLink>
              </NavItem>)
            }
            {AuthService.isAuthenticated() ? (
              <UncontrolledDropdown nav inNavbar className="ms-auto">
                <DropdownToggle nav caret>
                  {AuthService.getCurrentUser().email}
                </DropdownToggle>
                <DropdownMenu end>
                  {AuthService.isUser() && (
                    <DropdownItem>
                      <NavLink href={`/Account/AddFunds?id=${AuthService.getCurrentUser().id}`}>{t("Add funds")}</NavLink>
                    </DropdownItem>)}
                  <DropdownItem>
                    <NavLink href={`/Account/EditUser?id=${AuthService.getCurrentUser().id}`}>{t("Edit profile")}</NavLink>
                  </DropdownItem>
                  <DropdownItem>
                    <NavLink href={`/Account/ChangePassword?id=${AuthService.getCurrentUser().id}`}>{t("Change password")}</NavLink>
                  </DropdownItem>
                  <DropdownItem divider />
                  <DropdownItem>
                    <NavLink href="/" onClick={() => logout()}>{t("Logout")}</NavLink>
                  </DropdownItem>
                </DropdownMenu>
              </UncontrolledDropdown>) : (
              <Nav className="ms-auto">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Account/Login">{t("Login")}</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Account/Register">{t("Register")}</NavLink>
                </NavItem>
              </Nav>)
            }
          </Nav>
        </Collapse>
      </Navbar>
    </div>
  );
}