/*
    libember -- C++ 03 implementation of the Ember+ Protocol
    Copyright (C) 2012  L-S-B Broadcast Technologies GmbH

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

#ifndef __LIBEMBER_GLOW_GLOWQUALIFIEDPARAMETER_HPP
#define __LIBEMBER_GLOW_GLOWQUALIFIEDPARAMETER_HPP

#include "../ber/ObjectIdentifier.hpp"
#include "GlowParameterBase.hpp"

namespace libember { namespace glow
{
    /** Forward declarations **/
    class GlowNodeFactory;
    class GlowRootElementCollection;

    /**
     * Class representing a parameter description which contains a path.
     * The methods to access a property of this object return a default value if a property doesn't exist.
     * To assure that the property exists, the contains method should be used.
     */
    class LIBEMBER_API GlowQualifiedParameter : public GlowParameterBase
    {
        friend class GlowNodeFactory;
        public:
            /**
             * Initializes a parameter with the provided number.
             * @param path The path of this parameter.
             * @note The application tag will be set to ContextSpecific-0.
             */
            explicit GlowQualifiedParameter(ber::ObjectIdentifier const& path);

            /**
             * Initializes a new parameter and inserts it to the provided element collection.
             * @param parent The root element collection to insert this parameter to.
             * @param path The path this node has in the gadget tree.
             */
            GlowQualifiedParameter(GlowRootElementCollection* parent, ber::ObjectIdentifier const& path);

            /**
             * Constructor that initializes a parameter with the specified application tag.
             * @param path The path of this parameter.
             * @param tag Application tag.
             */
            GlowQualifiedParameter(ber::ObjectIdentifier const& path, ber::Tag const& tag);

            /**
             * Returns the path of this parameter.
             * @return The path of this parameter or an empty path, if not set.
             */
            ber::ObjectIdentifier const& path() const;

        protected:
            /** 
             * @see Container::insertImpl() 
             * @remarks This override resets the cached path.
             */
            virtual iterator insertImpl(iterator where, Node* child);

            /** 
             * @see Container::eraseImpl() 
             * @remarks This override resets the cached path.
             */
            virtual void eraseImpl(iterator first, iterator last);

        private:
            /**
             * This constructor initializes a parameter without the mandatory number.
             * It is only used by the NodeFactory since the number has not been decoded
             * when this container is being created.
             * @param tag Decoded application tag.
             */
            explicit GlowQualifiedParameter(ber::Tag const& tag);

        private:
            mutable ber::ObjectIdentifier m_cachedPath;
    };
}
}

#ifdef LIBEMBER_HEADER_ONLY
#  include "impl/GlowQualifiedParameter.ipp"
#endif

#endif  // __LIBEMBER_GLOW_GLOWQUALIFIEDPARAMETER_HPP

